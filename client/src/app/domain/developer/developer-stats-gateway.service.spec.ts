import { TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';

import { DeveloperStatsGatewayService } from './developer-stats-gateway.service';

describe('DeveloperStatsGatewayService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientModule
    ]
  }));

  it('should be created', () => {
    const service: DeveloperStatsGatewayService = TestBed.get(DeveloperStatsGatewayService);
    expect(service).toBeTruthy();
  });
});
