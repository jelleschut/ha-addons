import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { OpeningDayDto } from '../../models/models';

@Component({
  selector: 'app-opening-badge',
  standalone: true,
  imports: [CommonModule, MatChipsModule],
  template: `
    <!-- ng-container rendert geen enkel DOM-element, maar groepeert de *ngIf
         zodat we de null-check op één plek doen voor beide spans hieronder. -->
    <ng-container *ngIf="opening">
      <span *ngIf="opening.isOpen" class="opening-chip open">
        <!-- | slice:0:5 knipt de tijdstring "HH:MM:SS" in op "HH:MM".
             Dit is een ingebouwde Angular-pipe uit CommonModule. -->
        Open {{ opening.openTime | slice:0:5 }}–{{ opening.closeTime | slice:0:5 }}
      </span>
      <span *ngIf="!opening.isOpen" class="opening-chip closed">
        Gesloten
      </span>
    </ng-container>
  `,
  styles: [`
    .opening-chip {
      display: inline-flex;
      align-items: center;
      padding: 2px 8px;
      border-radius: 16px;
      font-size: 11px;
      font-weight: 500;
    }
    .open {
      background-color: #4CAF50;
      color: #fff;
    }
    .closed {
      border: 1px solid #9E9E9E;
      color: #9E9E9E;
      background: transparent;
    }
  `]
})
export class OpeningBadgeComponent {
  @Input() opening: OpeningDayDto | null = null;
}
