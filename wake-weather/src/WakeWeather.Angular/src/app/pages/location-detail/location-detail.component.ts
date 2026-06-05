import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatExpansionModule } from '@angular/material/expansion';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LocationDetailDto, WeatherResponse, ForecastDayDto, OpeningDayDto, HourlyWeatherDto } from '../../models/models';
import { WeatherApiService } from '../../services/weather-api.service';
import { FavoritesService } from '../../services/favorites.service';
import { WeatherSummaryComponent } from '../../components/weather-summary/weather-summary.component';
import { WakeScoreBadgeComponent } from '../../components/wake-score-badge/wake-score-badge.component';
import { OpeningBadgeComponent } from '../../components/opening-badge/opening-badge.component';
import { HourlyWeatherRowComponent } from '../../components/hourly-weather-row/hourly-weather-row.component';
import { beaufort } from '../../services/wind.helper';

@Component({
  selector: 'app-location-detail',
  standalone: true,
  imports: [
    CommonModule, RouterModule,
    MatButtonModule, MatIconModule, MatCardModule,
    MatProgressBarModule, MatExpansionModule,
    WeatherSummaryComponent, WakeScoreBadgeComponent,
    OpeningBadgeComponent, HourlyWeatherRowComponent
  ],
  template: `
    <div *ngIf="loading" class="loading">
      <div class="skeleton title-skeleton"></div>
      <div class="skeleton content-skeleton"></div>
    </div>

    <div *ngIf="!loading && !location" class="error-msg">
      Locatie niet gevonden.
    </div>

    <!-- ng-container groepeert de *ngIf zonder extra DOM-element toe te voegen. -->
    <ng-container *ngIf="!loading && location">
      <div class="page-header">
        <!-- routerLink als attribuut (zonder binding) werkt voor vaste paden. -->
        <button mat-icon-button routerLink="/">
          <mat-icon>arrow_back</mat-icon>
        </button>
        <h2 class="location-title">{{ location.name }}</h2>
        <div style="flex:1"></div>
        <button mat-icon-button (click)="toggleFavorite()">
          <mat-icon [style.color]="isFavorite ? '#F44336' : ''">
            {{ isFavorite ? 'favorite' : 'favorite_border' }}
          </mat-icon>
        </button>
      </div>
      <p class="location-city">{{ location.city }}</p>

      <h3 class="section-title">Nu</h3>
      <mat-card class="current-card">
        <mat-card-content>
          <app-weather-summary [weather]="weather?.current ?? null"/>
        </mat-card-content>
      </mat-card>

      <!-- ?. (optional chaining) voorkomt een runtime-fout als weather null is. -->
      <ng-container *ngIf="weather?.forecast?.length">
        <h3 class="section-title">Voorspelling</h3>

        <!-- trackBy: trackByDate voorkomt onnodige DOM-rebuilds bij change detection. -->
        <div *ngFor="let day of weather!.forecast; trackBy: trackByDate" class="day-card">
          <div class="day-header"
               [class.clickable]="hasHourly(day.date)"
               (click)="toggleDay(day)">
            <div>
              <div class="day-name">{{ dayLabel(day.date) }}</div>
              <div class="day-date">{{ formatDate(day.date) }}</div>
            </div>
            <div class="day-right">
              <app-opening-badge [opening]="openingForDay(day.date)"/>
              <app-wake-score-badge [score]="day.wakeScore"/>
              <mat-icon *ngIf="hasHourly(day.date)" class="expand-icon">
                {{ expandedDay === day.date ? 'expand_less' : 'expand_more' }}
              </mat-icon>
            </div>
          </div>

          <div class="day-grid">
            <div class="day-item">
              <mat-icon class="icon temp-icon">thermostat</mat-icon>
              <span>{{ day.tempMin }}° – {{ day.tempMax }}°C</span>
            </div>
            <div class="day-item">
              <mat-icon class="icon wind-icon">air</mat-icon>
              <span>{{ beaufort(day.avgWindSpeedKmh) }}</span>
              <span class="caption">(max {{ beaufort(day.maxWindSpeedKmh) }})</span>
            </div>
            <div class="day-item">
              <mat-icon class="icon rain-icon">water_drop</mat-icon>
              <span>{{ day.precipitationSumMm }} mm</span>
              <span class="caption">{{ day.maxPrecipitationProbability }}%</span>
            </div>
            <div class="day-item">
              <mat-icon class="icon cloud-icon">cloud</mat-icon>
              <span>{{ day.avgCloudCoverPercent }}%</span>
            </div>
          </div>

          <!-- Uurdata wordt alleen getoond als de dag uitgeklapt is EN de drempel
               van 5 dagen niet overschreden is. De data wordt lazy geladen:
               pas op het moment dat de gebruiker een dag uitklapt, wordt de
               HTTP-aanvraag gedaan. Het resultaat wordt gecached in hourlyCache. -->
          <div *ngIf="hasHourly(day.date) && expandedDay === day.date" class="hourly-section">
            <!-- null in de cache betekent: aanvraag loopt nog. -->
            <mat-progress-bar *ngIf="hourlyCache[day.date] === null" mode="indeterminate" color="primary"/>
            <!-- "as hours" slaat de waarde op als template-variabele en werkt
                 tegelijk als guard: de *ngIf is false als hours undefined is. -->
            <ng-container *ngIf="hourlyCache[day.date] as hours">
              <p *ngIf="hours.length === 0" class="closed-msg">Park is gesloten op deze dag.</p>
              <app-hourly-weather-row *ngFor="let h of hours" [hour]="h"/>
            </ng-container>
          </div>
        </div>
      </ng-container>

      <a *ngIf="location.website"
         [href]="location.website"
         target="_blank"
         mat-stroked-button color="primary"
         class="website-btn">
        <mat-icon>open_in_new</mat-icon>
        Website
      </a>
    </ng-container>
  `,
  styles: [`
    .loading { padding: 16px 0; }
    .skeleton {
      background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
      border-radius: 4px;
      margin-bottom: 8px;
    }
    .title-skeleton { height: 40px; width: 60%; }
    .content-skeleton { height: 200px; }
    .error-msg { color: #F44336; text-align: center; padding: 20px; }
    .page-header {
      display: flex;
      align-items: center;
      margin-bottom: 4px;
    }
    .location-title {
      margin: 0;
      font-size: 20px;
      font-weight: 500;
    }
    .location-city {
      font-size: 12px;
      color: rgba(0,0,0,0.6);
      margin: 0 0 16px 40px;
    }
    .section-title {
      font-size: 16px;
      font-weight: 500;
      margin: 0 0 8px 0;
    }
    .current-card {
      border-radius: 12px;
      margin-bottom: 16px;
    }
    .day-card {
      background: #fff;
      border-radius: 12px;
      padding: 12px;
      margin-bottom: 8px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.12);
    }
    .day-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 8px;
    }
    .day-header.clickable { cursor: pointer; }
    .day-name { font-size: 15px; font-weight: 600; }
    .day-date { font-size: 12px; color: rgba(0,0,0,0.6); }
    .day-right {
      display: flex;
      align-items: center;
      gap: 6px;
    }
    .expand-icon { font-size: 20px; color: rgba(0,0,0,0.5); }
    .day-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 6px;
    }
    .day-item {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 13px;
    }
    .icon { font-size: 16px; height: 16px; width: 16px; }
    .temp-icon { color: #FF9800; }
    .wind-icon { color: #2196F3; }
    .rain-icon { color: #1565C0; }
    .cloud-icon { color: #757575; }
    .caption { font-size: 11px; color: rgba(0,0,0,0.5); }
    .hourly-section { margin-top: 8px; border-top: 1px solid rgba(0,0,0,0.1); padding-top: 8px; }
    .closed-msg { font-size: 12px; color: rgba(0,0,0,0.5); margin: 0; }
    .website-btn { width: 100%; margin-top: 8px; justify-content: center; }
  `]
})
export class LocationDetailComponent implements OnInit {
  location: LocationDetailDto | null = null;
  weather: WeatherResponse | null = null;
  loading = true;
  isFavorite = false;
  expandedDay: string | null = null;

  // Record<string, ... | null> werkt als een dictionary voor lazy-loaded uurdata.
  // null = aanvraag loopt, [] = geen uurdata (park gesloten), array = geladen data.
  // Sleutel ontbreekt helemaal = aanvraag nog niet gestart.
  hourlyCache: Record<string, HourlyWeatherDto[] | null> = {};

  beaufort = beaufort;

  private slug = '';

  // ActivatedRoute geeft toegang tot de URL-parameters van de huidige route.
  // De slug uit /locatie/:slug wordt hiermee uitgelezen in ngOnInit.
  constructor(
    private route: ActivatedRoute,
    private api: WeatherApiService,
    private favoritesService: FavoritesService
  ) {}

  ngOnInit(): void {
    this.slug = this.route.snapshot.paramMap.get('slug') ?? '';

    // forkJoin voert beide HTTP-aanvragen parallel uit en wacht tot beide klaar zijn
    // voordat de callback wordt aangeroepen — vergelijkbaar met Promise.all().
    // catchError(() => of(null)) voorkomt dat een mislukte aanvraag de hele forkJoin
    // laat falen: in plaats daarvan krijgt die aanvraag null als resultaat.
    forkJoin({
      location: this.api.getLocation(this.slug).pipe(catchError(() => of(null))),
      weather: this.api.getWeather(this.slug).pipe(catchError(() => of(null)))
    }).subscribe(({ location, weather }) => {
      this.location = location as LocationDetailDto | null;
      this.weather = weather as WeatherResponse | null;
      if (this.location) {
        this.isFavorite = this.favoritesService.isFavorite(this.location.id);
      }
      this.loading = false;
    });
  }

  toggleFavorite(): void {
    if (this.location) {
      this.favoritesService.toggle(this.location.id);
      this.isFavorite = this.favoritesService.isFavorite(this.location.id);
    }
  }

  hasHourly(dateStr: string): boolean {
    const today = new Date(); today.setHours(0, 0, 0, 0);
    const date = new Date(dateStr + 'T00:00:00');
    const diff = Math.round((date.getTime() - today.getTime()) / 86400000);
    return diff <= 4;
  }

  toggleDay(day: ForecastDayDto): void {
    if (!this.hasHourly(day.date)) return;

    if (this.expandedDay === day.date) {
      this.expandedDay = null;
      return;
    }

    this.expandedDay = day.date;

    // Laad uurdata alleen als die nog niet eerder is opgehaald (lazy loading).
    // 'in'-operator controleert of de sleutel bestaat (ook als de waarde null is).
    if (!(day.date in this.hourlyCache)) {
      this.hourlyCache[day.date] = null; // markeer als "bezig met laden"
      this.api.getHourlyWeather(this.slug, day.date).subscribe({
        next: hours => { this.hourlyCache[day.date] = hours; },
        error: () => { this.hourlyCache[day.date] = []; }
      });
    }
  }

  openingForDay(dateStr: string): OpeningDayDto | null {
    return this.location?.weekOpening.find(o => o.date === dateStr) ?? null;
  }

  dayLabel(dateStr: string): string {
    const today = new Date(); today.setHours(0, 0, 0, 0);
    // 'T00:00:00' dwingt lokale tijdzone af; zonder dit interpreteert JavaScript
    // een datum als "2025-06-04" als middernacht UTC, wat in tijdzone UTC+2 één
    // dag terug geeft (3 juni 22:00 lokaal).
    const date = new Date(dateStr + 'T00:00:00');
    const diff = Math.round((date.getTime() - today.getTime()) / 86400000);
    if (diff === 0) return 'Vandaag';
    if (diff === 1) return 'Morgen';
    return date.toLocaleDateString('nl-NL', { weekday: 'short', day: 'numeric', month: 'short' });
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr + 'T00:00:00');
    return date.toLocaleDateString('nl-NL', { day: '2-digit', month: 'short' });
  }

  trackByDate(_: number, day: ForecastDayDto): string {
    return day.date;
  }
}
