import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LocationSummaryDto, WeatherResponse, ForecastDayDto } from '../../models/models';
import { WeatherApiService } from '../../services/weather-api.service';
import { FavoritesService } from '../../services/favorites.service';
import { WeatherSummaryComponent } from '../weather-summary/weather-summary.component';
import { WakeScoreBadgeComponent } from '../wake-score-badge/wake-score-badge.component';
import { OpeningBadgeComponent } from '../opening-badge/opening-badge.component';

@Component({
  selector: 'app-location-card',
  standalone: true,
  // RouterModule is nodig voor de [routerLink]-directive in de template.
  // Zonder deze import kent Angular [routerLink] niet en volgt een compile-fout.
  imports: [
    CommonModule, RouterModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatProgressBarModule, MatProgressSpinnerModule,
    WeatherSummaryComponent, WakeScoreBadgeComponent, OpeningBadgeComponent
  ],
  template: `
    <mat-card class="location-card">
      <mat-card-header>
        <div class="header-row">
          <div>
            <h3 class="location-name">{{ location.name }}</h3>
            <span class="location-city">{{ location.city }}</span>
          </div>
          <div class="header-actions">
            <app-opening-badge [opening]="location.todayOpening"/>
            <button mat-icon-button (click)="toggleFavorite($event)" class="fav-btn">
              <mat-icon [style.color]="isFavorite ? '#F44336' : ''">
                {{ isFavorite ? 'favorite' : 'favorite_border' }}
              </mat-icon>
            </button>
          </div>
        </div>
      </mat-card-header>

      <mat-card-content>
        <div *ngIf="loading" class="loading">
          <mat-progress-bar mode="indeterminate" color="primary"/>
          <div class="skeleton"></div>
        </div>

        <div *ngIf="!loading && error" class="error-msg">
          Weer kon niet worden geladen
        </div>

        <!-- ng-container rendert geen DOM-element, maar groepeert de *ngIf
             voor meerdere child-elementen zonder extra wrapper in de HTML. -->
        <ng-container *ngIf="!loading && !error && weather">
          <!-- ?? kiest het eerste niet-null alternatief: laat openingstijdenweer
               zien als dat beschikbaar is, anders het actuele weer. -->
          <app-weather-summary [weather]="weather.todayOpeningWeather ?? weather.current"/>

          <div class="scores-row">
            <div *ngIf="todayScore !== null" class="score-item">
              <span class="score-label">Vandaag:</span>
              <app-wake-score-badge [score]="todayScore"/>
            </div>
            <!-- "as best" slaat het resultaat van de getter op als lokale variabele
                 zodat we hem niet twee keer opvragen binnen dit blok. -->
            <div *ngIf="bestForecastDay as best" class="score-item">
              <span class="score-label">Beste dag:</span>
              <span class="best-label">{{ best.label }}</span>
              <app-wake-score-badge [score]="best.day.wakeScore"/>
            </div>
          </div>
        </ng-container>
      </mat-card-content>

      <mat-card-actions>
        <!-- [routerLink] navigeert via Angular's router (geen volledige page-reload).
             Array-syntax zorgt dat de router de segmenten correct samenvoegt. -->
        <button mat-button color="primary" [routerLink]="['/locatie', location.slug]">
          14-daagse voorspelling
          <mat-icon>chevron_right</mat-icon>
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .location-card {
      border-radius: 16px;
      margin-bottom: 12px;
    }
    .header-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      width: 100%;
    }
    .location-name {
      margin: 0;
      font-size: 18px;
      font-weight: 500;
    }
    .location-city {
      font-size: 12px;
      color: rgba(0,0,0,0.6);
    }
    .header-actions {
      display: flex;
      align-items: center;
      gap: 4px;
    }
    .fav-btn { color: rgba(0,0,0,0.54); }
    .loading { padding: 8px 0; }
    .skeleton {
      height: 60px;
      background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
      border-radius: 4px;
      margin-top: 8px;
    }
    .error-msg {
      font-size: 13px;
      color: #FF9800;
      padding: 4px 0;
    }
    .scores-row {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-top: 8px;
    }
    .score-item {
      display: flex;
      align-items: center;
      gap: 4px;
    }
    .score-label {
      font-size: 12px;
      color: rgba(0,0,0,0.6);
    }
    .best-label {
      font-size: 12px;
    }
  `]
})
// implements OnInit verplicht de klasse om ngOnInit() te implementeren.
// Dit is geen harde vereiste van Angular, maar maakt de intentie expliciet en
// geeft TypeScript de kans om een typefout te melden als de naam verkeerd is.
export class LocationCardComponent implements OnInit {
  // required: true zorgt voor een compile-fout als de parent [location] niet meegeeft.
  @Input({ required: true }) location!: LocationSummaryDto;

  // @Output() + EventEmitter maakt een aangepast event dat de parent kan afluisteren
  // via (favoriteChanged)="...". Zo communiceert een child omhoog naar zijn parent.
  @Output() favoriteChanged = new EventEmitter<void>();

  weather: WeatherResponse | null = null;
  loading = true;
  error = false;
  isFavorite = false;

  // Services worden via de constructor geïnjecteerd. Angular's DI-systeem levert
  // de instanties op basis van de type-annotaties. De private-modifier zorgt
  // dat ze alleen binnen deze klasse toegankelijk zijn.
  constructor(
    private api: WeatherApiService,
    private favoritesService: FavoritesService
  ) {}

  // ngOnInit() wordt aangeroepen nadat Angular alle @Input-properties heeft gezet.
  // HTTP-aanroepen horen hier, niet in de constructor — in de constructor zijn
  // @Inputs nog niet beschikbaar.
  ngOnInit(): void {
    this.isFavorite = this.favoritesService.isFavorite(this.location.id);

    // Abonneer op de favorietenstroom zodat de hartkleur bijwerkt als een andere
    // kaart dezelfde locatie als favoriet markeert.
    this.favoritesService.favorites$.subscribe(favs => {
      this.isFavorite = favs.has(this.location.id);
    });

    this.api.getWeather(this.location.slug).subscribe({
      next: w => { this.weather = w; this.loading = false; },
      error: () => { this.error = true; this.loading = false; }
    });
  }

  toggleFavorite(event: Event): void {
    // stopPropagation voorkomt dat de klik doorborrelt naar de kaart eronder,
    // wat onbedoeld navigatie of andere click-handlers zou triggeren.
    event.stopPropagation();
    this.favoritesService.toggle(this.location.id);
    this.favoriteChanged.emit();
  }

  // Getter: berekend elke keer dat Angular de template controleert (change detection).
  // Geeft de score van vandaag terug; valt terug op currentScore voor parken
  // die al gesloten zijn (geen forecastdag voor vandaag beschikbaar).
  get todayScore(): number | null {
    const today = new Date().toISOString().slice(0, 10);
    const todayForecast = this.weather?.forecast.find(f => f.date === today);
    return todayForecast?.wakeScore ?? this.weather?.currentScore ?? null;
  }

  get bestForecastDay(): { day: ForecastDayDto; label: string } | null {
    if (!this.weather?.forecast?.length) return null;
    const best = this.weather.forecast
      .filter(f => f.wakeScore !== null)
      .reduce((a, b) => (b.wakeScore! > (a?.wakeScore ?? -1) ? b : a), null as ForecastDayDto | null);
    if (!best) return null;

    // Toon "beste dag" alleen als het niet vandaag is — anders is er overlap
    // met de "Vandaag"-score die al apart getoond wordt.
    const today = new Date().toISOString().slice(0, 10);
    if (best.date === today) return null;

    const label = this.dayLabel(best.date);
    return { day: best, label };
  }

  private dayLabel(dateStr: string): string {
    const today = new Date(); today.setHours(0, 0, 0, 0);
    // 'T00:00:00' voorkomt dat de browser de datum als UTC interpreteert en
    // daardoor één dag verschuift in tijdzones die voor UTC liggen.
    const date = new Date(dateStr + 'T00:00:00');
    const diff = Math.round((date.getTime() - today.getTime()) / 86400000);
    if (diff === 0) return 'Vandaag';
    if (diff === 1) return 'Morgen';
    return date.toLocaleDateString('nl-NL', { weekday: 'short', day: 'numeric', month: 'short' });
  }
}
