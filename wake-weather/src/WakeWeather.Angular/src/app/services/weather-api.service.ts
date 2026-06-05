import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LocationSummaryDto, LocationDetailDto, WeatherResponse, HourlyWeatherDto } from '../models/models';

// providedIn: 'root' maakt van deze service een singleton: er bestaat precies één
// instantie voor de hele app. Elke component of service die WeatherApiService
// injecteert, krijgt diezelfde instantie terug.
@Injectable({ providedIn: 'root' })
export class WeatherApiService {
  // HttpClient wordt via de constructor geïnjecteerd (dependency injection).
  // Angular weet hoe HttpClient aangemaakt moet worden dankzij provideHttpClient()
  // in app.config.ts.
  constructor(private http: HttpClient) {}

  // De URL's zijn relatief (/api/...) zodat zowel de dev-proxy (proxy.conf.json)
  // als de productie-nginx het verzoek kunnen doorsturen naar de API-container,
  // zonder dat de frontend weet op welk adres de API draait.

  // http.get<T>() geeft een Observable terug, geen Promise. De HTTP-aanvraag
  // wordt pas écht gedaan op het moment dat iemand .subscribe() aanroept.
  getLocations(): Observable<LocationSummaryDto[]> {
    return this.http.get<LocationSummaryDto[]>('/api/locations');
  }

  getLocation(slug: string): Observable<LocationDetailDto> {
    return this.http.get<LocationDetailDto>(`/api/locations/${slug}`);
  }

  getWeather(slug: string): Observable<WeatherResponse> {
    return this.http.get<WeatherResponse>(`/api/weather/${slug}`);
  }

  getHourlyWeather(slug: string, date: string): Observable<HourlyWeatherDto[]> {
    return this.http.get<HourlyWeatherDto[]>(`/api/weather/${slug}/hourly/${date}`);
  }
}
