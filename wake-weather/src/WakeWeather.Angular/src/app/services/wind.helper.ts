// Gewone functies in plaats van een service of Angular-pipe, omdat ze
// puur zijn (geen state, geen injection) en ook in templates van meerdere
// componenten gebruikt worden. Ze worden als class-properties toegewezen
// (zie WeatherSummaryComponent) zodat de template er bij kan.

export function beaufort(kmh: number): number {
  if (kmh < 1) return 0;
  if (kmh < 6) return 1;
  if (kmh < 12) return 2;
  if (kmh < 20) return 3;
  if (kmh < 29) return 4;
  if (kmh < 39) return 5;
  if (kmh < 50) return 6;
  if (kmh < 62) return 7;
  if (kmh < 75) return 8;
  if (kmh < 89) return 9;
  if (kmh < 103) return 10;
  if (kmh < 118) return 11;
  return 12;
}

export function directionLabel(deg: number): string {
  // Modulo-normalisatie: zorgt dat negatieve graden (bijv. -10°) correct
  // worden omgezet naar het positieve bereik (350°).
  const d = ((deg % 360) + 360) % 360;
  if (d < 22.5) return 'N';
  if (d < 67.5) return 'NO';
  if (d < 112.5) return 'O';
  if (d < 157.5) return 'ZO';
  if (d < 202.5) return 'Z';
  if (d < 247.5) return 'ZW';
  if (d < 292.5) return 'W';
  if (d < 337.5) return 'NW';
  return 'N'; // 337.5–360° valt terug op Noord
}
