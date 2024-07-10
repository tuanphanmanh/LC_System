import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FormStoringService {

  constructor() {}

  set(key, data) {
    localStorage.setItem(key, JSON.stringify(data));
  }

  get(key) {
    const val = localStorage.getItem(key);
    return val ? JSON.parse(val) : '';
  }

  clear(key) {
    localStorage.removeItem(key);
  }
}
