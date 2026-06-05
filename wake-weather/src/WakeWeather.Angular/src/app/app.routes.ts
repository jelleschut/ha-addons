import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LocationDetailComponent } from './pages/location-detail/location-detail.component';

// Alle routes van de app staan hier centraal gedefinieerd.
// Angular's router vergelijkt het huidige URL-pad van boven naar beneden
// en laadt het bijbehorende component in de <router-outlet>.
export const routes: Routes = [
  { path: '', component: HomeComponent },

  // :slug is een routeparameter; de waarde is opvraagbaar via ActivatedRoute.
  { path: 'locatie/:slug', component: LocationDetailComponent },

  // Wildcard-route als vangnet: elk onbekend pad stuurt terug naar de homepage.
  // Moet altijd als laatste staan, anders wordt hij altijd geraakt.
  { path: '**', redirectTo: '' }
];
