export abstract class BaseBuilder<T> {
  private actions: ((developerStats: T) => void)[]

  constructor() {
    this.actions = [];
  }

  public abstract createEntity(): T

  public with(action: (developerStats: T) => void): BaseBuilder<T> {
    this.actions.push(action)
    return this;
  }

  public build(): T {
    let entity = this.createEntity();

    this.actions.forEach(action => {
      action(entity);
    });

    return entity;
  }
}