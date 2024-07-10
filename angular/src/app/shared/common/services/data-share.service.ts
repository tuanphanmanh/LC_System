import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataShareService {

public userCount$ = new BehaviorSubject<number>(0);

constructor() { }

changeCount(message) {
    this.userCount$.next(message);
  }

}
