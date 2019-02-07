import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import { AnalysisStats } from '../analysisStats';

@Injectable({
  providedIn: 'root'
})
export class DeveloperStatsGatewayService {
  constructor(private http: HttpClient) { }

  public get(): Observable<AnalysisStats[]> {
    return this.http.get<AnalysisStats[]>('assets/test-data.json');
  }
}
