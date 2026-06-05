import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

const STORAGE_KEY = 'wakeweather-favorites';

// providedIn: 'root' zorgt ervoor dat er maar één instantie is voor de hele app.
// Zo delen alle LocationCard-componenten dezelfde favorietenlijst zonder data door
// te hoeven geven via @Input/@Output.
@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private ids = new Set<number>(this.load());

  // BehaviorSubject is een Observable die altijd zijn laatste waarde onthoudt.
  // Nieuwe subscribers ontvangen direct de huidige stand, zonder te hoeven wachten
  // op de volgende update. Dat is handig voor state die al bestaat vóór subscribe().
  private subject = new BehaviorSubject<Set<number>>(this.ids);

  // .asObservable() geeft een read-only Observable terug. Zo kunnen components
  // de stroom volgen maar niet zelf .next() aanroepen en de state bederven.
  favorites$ = this.subject.asObservable();

  get favoriteIds(): Set<number> {
    return this.ids;
  }

  isFavorite(id: number): boolean {
    return this.ids.has(id);
  }

  toggle(id: number): void {
    if (this.ids.has(id)) {
      this.ids.delete(id);
    } else {
      this.ids.add(id);
    }
    this.save();
    // new Set(this.ids) maakt een kopie. Als we this.ids direct doorgeven,
    // zien subscribers altijd hetzelfde object en detecteert Angular de wijziging
    // mogelijk niet (object-referentie verandert niet).
    this.subject.next(new Set(this.ids));
  }

  private load(): number[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? JSON.parse(raw) : [];
    } catch {
      // JSON.parse gooit een fout als localStorage corrupt data bevat.
      // Dan starten we gewoon met een lege lijst.
      return [];
    }
  }

  private save(): void {
    // Set is niet JSON-serialiseerbaar, dus we zetten hem om naar een array.
    localStorage.setItem(STORAGE_KEY, JSON.stringify([...this.ids]));
  }
}
