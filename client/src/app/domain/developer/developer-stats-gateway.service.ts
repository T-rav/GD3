import { Injectable } from '@angular/core';
import { DeveloperStats } from './developer-stats'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DeveloperStatsGatewayService {
  constructor(private http: HttpClient) { }

  public get(): Observable<DeveloperStats[]> {
    return this.http.get<DeveloperStats[]>('assets/test-data.json');
  }
}
