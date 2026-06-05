import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { WeatherDto } from '../../models/models';
import { beaufort, directionLabel } from '../../services/wind.helper';

@Component({
  selector: 'app-weather-summary',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div *ngIf="weather" class="weather-grid">
      <div class="weather-item">
        <mat-icon class="icon temp-icon">thermostat</mat-icon>
        <div>
          <span class="value">{{ weather.feelsLike }}°C</span>
          <span class="caption">gevoels</span>
        </div>
      </div>
      <div class="weather-item">
        <mat-icon class="icon wind-icon">air</mat-icon>
        <div>
          <!-- beaufort en directionLabel zijn class-properties (zie onderaan).
               Angular-templates kunnen alleen klasse-members aanroepen, geen
               module-level functies. Vandaar de toewijzing in de klasse. -->
          <span class="value">{{ beaufort(weather.windSpeedKmh) }} {{ directionLabel(weather.windDirectionDeg) }}</span>
          <span class="caption">bft</span>
        </div>
      </div>
      <div class="weather-item">
        <mat-icon class="icon rain-icon">water_drop</mat-icon>
        <div>
          <span class="value">{{ weather.precipitationMm }} mm</span>
          <span class="caption">{{ weather.precipitationProbability }}%</span>
        </div>
      </div>
      <div class="weather-item">
        <mat-icon class="icon cloud-icon">cloud</mat-icon>
        <div>
          <span class="value">{{ weather.cloudCoverPercent }}%</span>
          <span class="caption">bewolkt</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .weather-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 8px;
    }
    .weather-item {
      display: flex;
      align-items: center;
      gap: 6px;
    }
    .icon {
      font-size: 18px;
      height: 18px;
      width: 18px;
    }
    .temp-icon { color: #FF9800; }
    .wind-icon { color: #2196F3; }
    .rain-icon { color: #1565C0; }
    .cloud-icon { color: #757575; }
    .value {
      display: block;
      font-size: 14px;
      font-weight: 500;
    }
    .caption {
      display: block;
      font-size: 11px;
      color: rgba(0,0,0,0.6);
    }
  `]
})
export class WeatherSummaryComponent {
  @Input() weather: WeatherDto | null = null;

  // Hulpfuncties worden als class-properties toegewezen zodat de template
  // ze kan aanroepen als {{ beaufort(...) }}. Zonder deze toewijzing gooit
  // Angular een compilatiefout omdat de template buiten de klasse-scope kijkt.
  beaufort = beaufort;
  directionLabel = directionLabel;
}
