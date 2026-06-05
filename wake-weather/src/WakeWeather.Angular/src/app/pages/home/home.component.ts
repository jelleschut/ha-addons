import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { LocationSummaryDto } from '../../models/models';
import { WeatherApiService } from '../../services/weather-api.service';
import { FavoritesService } from '../../services/favorites.service';
import { LocationCardComponent } from '../../components/location-card/location-card.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressSpinnerModule, MatButtonToggleModule, MatIconModule, MatButtonModule,
    LocationCardComponent
  ],
  template: `
    <div class="page-header">
      <h1 class="page-title">WakeWeather</h1>
      <button mat-icon-button (click)="toggleFavoritesOnly()" [color]="favoritesOnly ? 'warn' : ''">
        <mat-icon>{{ favoritesOnly ? 'favorite' : 'favorite_border' }}</mat-icon>
      </button>
    </div>

    <div *ngIf="loading" class="spinner-wrap">
      <mat-spinner diameter="40"/>
    </div>

    <div *ngIf="error" class="error-msg">Locaties konden niet worden geladen.</div>

    <div *ngIf="!loading && !error">
      <!-- trackBy voorkomt dat Angular alle kaarten vernietigt en opnieuw aanmaakt
           bij elke herrendering (bijv. na een favoriet-toggle). Door elke locatie
           te identificeren op basis van id, hergebruikt Angular bestaande DOM-nodes
           en hoeven componenten hun data niet opnieuw op te laden. -->
      <app-location-card
        *ngFor="let loc of displayedLocations; trackBy: trackById"
        [location]="loc"
        (favoriteChanged)="onFavoriteChanged()"
      />
      <p *ngIf="favoritesOnly && displayedLocations.length === 0" class="empty-msg">
        Geen favorieten geselecteerd.
      </p>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    .page-title {
      margin: 0;
      font-size: 24px;
      font-weight: 600;
    }
    .spinner-wrap {
      display: flex;
      justify-content: center;
      padding: 40px 0;
    }
    .error-msg {
      color: #F44336;
      text-align: center;
      padding: 20px;
    }
    .empty-msg {
      text-align: center;
      color: rgba(0,0,0,0.5);
      padding: 24px;
    }
  `]
})
export class HomeComponent implements OnInit {
  locations: LocationSummaryDto[] = [];
  loading = true;
  error = false;
  favoritesOnly = false;

  constructor(
    private api: WeatherApiService,
    private favoritesService: FavoritesService
  ) {}

  ngOnInit(): void {
    this.api.getLocations().subscribe({
      next: locs => {
        this.locations = locs;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  // Getter i.p.v. een veld: wordt elke change-detection-cyclus opnieuw berekend.
  // Zo reflecteert de volgorde automatisch de actuele favorietenstand, ook nadat
  // een kaart zijn (favoriteChanged)-event uitstuurt en onFavoriteChanged() wordt
  // aangeroepen — dat triggert een re-render, waarna deze getter opnieuw loopt.
  get displayedLocations(): LocationSummaryDto[] {
    const favIds = this.favoritesService.favoriteIds;
    let list = this.favoritesOnly
      ? this.locations.filter(l => favIds.has(l.id))
      : [...this.locations];

    return list.sort((a, b) => {
      const aFav = favIds.has(a.id) ? 0 : 1;
      const bFav = favIds.has(b.id) ? 0 : 1;
      if (aFav !== bFav) return aFav - bFav;
      return a.name.localeCompare(b.name);
    });
  }

  toggleFavoritesOnly(): void {
    this.favoritesOnly = !this.favoritesOnly;
  }

  // Deze methode heeft geen body nodig: het feit dat hij aangeroepen wordt,
  // markeert de component als "dirty" in Angular's change detection, waardoor
  // de displayedLocations-getter bij de volgende tick opnieuw wordt berekend.
  onFavoriteChanged(): void {}

  // trackBy-functie voor *ngFor: geeft een stabiele identifier per item terug.
  // Angular gebruikt dit om DOM-elementen te herkennen bij volgorde-wijzigingen.
  // De eerste parameter (index) is verplicht door het interface maar ongebruikt.
  trackById(_: number, loc: LocationSummaryDto): number {
    return loc.id;
  }
}
