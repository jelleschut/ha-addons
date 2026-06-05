import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-wake-score-badge',
  standalone: true,
  // Standalone components importeren alleen wat ze zelf gebruiken.
  // CommonModule levert *ngIf, *ngFor en pipes zoals | slice.
  // Elke Material-module wordt los geïmporteerd i.p.v. één grote MaterialModule,
  // zodat de tree-shaker ongebruikte Material-code uit de bundel kan weggooien.
  imports: [CommonModule, MatChipsModule, MatIconModule],
  template: `
    <span *ngIf="score !== null && score !== undefined"
          class="wake-score-chip"
          [style.background-color]="chipColor"
          [style.color]="'#fff'">
      <mat-icon style="font-size:14px;height:14px;width:14px;vertical-align:middle;">surfing</mat-icon>
      {{ score.toFixed(1) }}
    </span>
  `,
  styles: [`
    .wake-score-chip {
      display: inline-flex;
      align-items: center;
      gap: 2px;
      padding: 2px 8px;
      border-radius: 16px;
      font-size: 12px;
      font-weight: 500;
    }
  `]
})
export class WakeScoreBadgeComponent {
  // @Input() declareert een eigenschap die de parent-component van buiten kan
  // instellen via [score]="...". null als default zodat de badge niets toont
  // als er nog geen score beschikbaar is.
  @Input() score: number | null = null;

  // Een getter werkt goed voor afgeleide weergavelogica: hij wordt herberekend
  // elke keer dat Angular de template opnieuw rendert (change detection).
  get chipColor(): string {
    if (this.score === null) return '#9E9E9E';
    if (this.score >= 8) return '#4CAF50';
    if (this.score >= 6) return '#FFC107';
    if (this.score >= 4) return '#FF9800';
    return '#F44336';
  }
}
