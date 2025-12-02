import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FeedbackListComponent } from './feedback-list.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ApiServiceService } from '../../services/api-service.service';

describe('FeedbackListComponent', () => {
  let component: FeedbackListComponent;
  let fixture: ComponentFixture<FeedbackListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        FeedbackListComponent,
        RouterTestingModule.withRoutes([]),
        HttpClientTestingModule,
      ],
      providers: [ApiServiceService],
    }).compileComponents();

    fixture = TestBed.createComponent(FeedbackListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
