import { BaseBuilder } from './base-builder'
import { DeveloperStats } from './../app/domain/developer/developer-stats';
import * as faker from 'faker';

export class DeveloperStatsTestDataBuilder extends BaseBuilder<DeveloperStats> {
  public static create(): DeveloperStatsTestDataBuilder {
    return new DeveloperStatsTestDataBuilder();
  }

  public createEntity(): DeveloperStats {
    return {
      author: { name: faker.name.firstName(), emails: [faker.internet.email()] },
      periodActiveDays: faker.random.number(),
      activeDaysPerWeek: faker.random.number(),
      commitsPerDay: faker.random.number(),
      impact: faker.random.number(),
      linesOfChangePerHour: faker.random.number(),
      churn: faker.random.number(),
      linesAdded: faker.random.number(),
      linesRemoved: faker.random.number(),
      rtt100: faker.random.number(),
      ptt100: faker.random.number(),
      dtt100: faker.random.number(),
      riskFactor: faker.random.number()
    };
  }
}