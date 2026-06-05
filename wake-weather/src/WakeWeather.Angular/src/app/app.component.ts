import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// De root-component is het startpunt van de app. Hij wordt ingeladen via de
// <app-root>-tag in index.html. Zijn enige taak is de shell-layout bieden
// en de <router-outlet> renderen — de plek waar de actieve page-component
// door de router wordt ingeladen op basis van het huidige URL-pad.
@Component({
  selector: 'app-root',
  // standalone: true betekent dat deze component geen NgModule nodig heeft.
  // In plaats daarvan declareert hij zelf welke andere componenten, directives
  // en pipes hij gebruikt via de imports-array hieronder.
  standalone: true,
  // RouterOutlet is de enige benodigde import: het is de Angular-directive die
  // de <router-outlet>-tag in de template herkent en activeer.
  imports: [RouterOutlet],
  template: `
    <div class="app-container">
      <router-outlet/>
    </div>
  `,
  styles: [`
    .app-container {
      max-width: 600px;
      margin: 0 auto;
      padding: 16px;
    }
  `]
})
export class AppComponent {}
