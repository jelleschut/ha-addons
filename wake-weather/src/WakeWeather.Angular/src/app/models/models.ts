export interface OpeningDayDto {
  date: string;
  isOpen: boolean;
  openTime: string | null;
  closeTime: string | null;
  note: string | null;
}

export interface LocationSummaryDto {
  id: number;
  name: string;
  slug: string;
  latitude: number;
  longitude: number;
  website: string | null;
  city: string;
  todayOpening: OpeningDayDto;
}

export interface LocationDetailDto {
  id: number;
  name: string;
  slug: string;
  latitude: number;
  longitude: number;
  website: string | null;
  city: string;
  weekOpening: OpeningDayDto[];
}

export interface WeatherDto {
  temperature: number;
  feelsLike: number;
  windSpeedKmh: number;
  windDirectionDeg: number;
  precipitationProbability: number;
  precipitationMm: number;
  cloudCoverPercent: number;
  updatedAt: string;
}

export interface ForecastDayDto {
  date: string;
  tempMin: number;
  tempMax: number;
  avgWindSpeedKmh: number;
  maxWindSpeedKmh: number;
  precipitationSumMm: number;
  maxPrecipitationProbability: number;
  avgCloudCoverPercent: number;
  wakeScore: number | null;
}

export interface WeatherResponse {
  current: WeatherDto | null;
  forecast: ForecastDayDto[];
  currentScore: number | null;
  todayOpeningWeather: WeatherDto | null;
}

export interface HourlyWeatherDto {
  time: string;
  temperature: number;
  feelsLike: number;
  windSpeedKmh: number;
  windDirectionDeg: number;
  precipitationProbability: number;
  precipitationMm: number;
  cloudCoverPercent: number;
  wakeScore: number;
}
