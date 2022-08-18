import { Ds2019sModule } from './ds2019s.module';

describe('Ds2019sModule', () => {
  let ds2019Module: Ds2019sModule;

  beforeEach(() => {
    ds2019Module = new Ds2019sModule();
  });

  it('should create an instance', () => {
    expect(ds2019Module).toBeTruthy();
  });
});
