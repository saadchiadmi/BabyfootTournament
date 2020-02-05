import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OneTournamentComponent } from './one-tournament.component';
import 'jasmine'

describe('OneTournamentComponent', () => {
  let component: OneTournamentComponent;
  let fixture: ComponentFixture<OneTournamentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OneTournamentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OneTournamentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
