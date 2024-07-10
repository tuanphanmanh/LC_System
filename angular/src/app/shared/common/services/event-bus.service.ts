import { Params } from '@angular/router';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, Observer } from 'rxjs';
import { filter } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class EventBusService {
    private event = new Subject<any>();
    private paramsSubject = new BehaviorSubject<any>(null);

    emit(data: any, params?:any) {
        this.event.next(data);
        if(data) this.paramsSubject.next(data);
    }

    on(eventType: (any)): Observable<any> {
        return this.event.pipe(filter(t => t.type === eventType));
    }

    clearObservers() {
        this.event.observers = [];
        this.paramsSubject.observers = [];
    }

    setData(data: any) {
        // Object.assign(this.paramsSubject.getValue(),{[propName] : data});
        this.paramsSubject.next(data);
    }
    getData() {
        return this.paramsSubject.getValue();
    }

}
