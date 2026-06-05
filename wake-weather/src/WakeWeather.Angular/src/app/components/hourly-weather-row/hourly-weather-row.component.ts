import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { HourlyWeatherDto } from '../../models/models';
import { WakeScoreBadgeComponent } from '../wake-score-badge/wake-score-badge.component';
import { beaufort, directionLabel } from '../../services/wind.helper';

@Component({
  selector: 'app-hourly-weather-row',
  standalone: true,
  // Standalone components importeren andere standalone components direct —
  // geen tussenliggend NgModule nodig.
  imports: [CommonModule, MatIconModule, WakeScoreBadgeComponent],
  template: `
    <div class="hour-row">
      <div class="time-score">
        <!-- | slice:0:5 knipt "HH:MM:SS" in op "HH:MM" -->
        <span class="time">{{ hour.time | slice:0:5 }}</span>
        <app-wake-score-badge [score]="hour.wakeScore"/>
      </div>
      <div class="hour-grid">
        <div class="hour-item">
          <mat-icon class="icon temp-icon">thermostat</mat-icon>
          <span>{{ hour.feelsLike }}°</span>
        </div>
        <div class="hour-item">
          <mat-icon class="icon wind-icon">air</mat-icon>
          <span>{{ beaufort(hour.windSpeedKmh) }} {{ directionLabel(hour.windDirectionDeg) }}</span>
        </div>
        <div class="hour-item">
          <mat-icon class="icon rain-icon">water_drop</mat-icon>
          <span>{{ hour.precipitationMm }} mm</span>
          <span class="caption">{{ hour.precipitationProbability }}%</span>
        </div>
        <div class="hour-item">
          <mat-icon class="icon cloud-icon">cloud</mat-icon>
          <span>{{ hour.cloudCoverPercent }}%</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .hour-row {
      padding: 6px 0;
      border-bottom: 1px solid rgba(0,0,0,0.08);
    }
    .hour-row:last-child { border-bottom: none; }
    .time-score {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 4px;
    }
    .time {
      font-size: 13px;
      font-weight: 600;
      min-width: 36px;
    }
    .hour-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 4px;
      padding-left: 4px;
    }
    .hour-item {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 12px;
    }
    .icon {
      font-size: 14px;
      height: 14px;
      width: 14px;
    }
    .temp-icon { color: #FF9800; }
    .wind-icon { color: #2196F3; }
    .rain-icon { color: #1565C0; }
    .cloud-icon { color: #757575; }
    .caption {
      font-size: 11px;
      color: rgba(0,0,0,0.5);
    }
  `]
})
export class HourlyWeatherRowComponent {
  // required: true genereert een compile-fout als de parent dit attribuut vergeet.
  // Het uitroepteken (!) onderdruk de TypeScript "possibly undefined"-waarschuwing;
  // dat is veilig omdat Angular garandeert dat required inputs gezet zijn vóór
  // ngOnInit en de template worden uitgevoerd.
  @Input({ required: true }) hour!: HourlyWeatherDto;

  beaufort = beaufort;
  directionLabel = directionLabel;
}
