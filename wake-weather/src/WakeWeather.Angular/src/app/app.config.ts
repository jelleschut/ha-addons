import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app.routes';

// ApplicationConfig is de moderne manier (Angular 17+) om de app te configureren
// zonder een NgModule. Vroeger stond dit in AppModule met imports/providers arrays.
// Elke provide*-functie registreert een dienst globaal in de dependency injection tree.
export const appConfig: ApplicationConfig = {
  providers: [
    // Registreert de router op basis van de routes in app.routes.ts.
    provideRouter(routes),

    // Maakt HttpClient beschikbaar voor dependency injection in services.
    // Zonder deze regel gooit Angular een fout als een service HttpClient injecteert.
    provideHttpClient(),

    // Angular Material-componenten gebruiken animaties (uitklap, ripple, etc.).
    // provideAnimations() activeert BrowserAnimationsModule globaal.
    // Alternatief: provideNoopAnimations() schakelt animaties uit (bijv. voor tests).
    provideAnimations()
  ]
};
