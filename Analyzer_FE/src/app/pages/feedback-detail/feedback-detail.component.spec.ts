import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FeedbackDetailComponent } from './feedback-detail.component';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ApiServiceService } from '../../services/api-service.service';
import { of } from 'rxjs';

describe('FeedbackDetailComponent', () => {
  let component: FeedbackDetailComponent;
  let fixture: ComponentFixture<FeedbackDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeedbackDetailComponent, HttpClientTestingModule],
      providers: [
        ApiServiceService,
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ id: '123' })),
            snapshot: {
              paramMap: convertToParamMap({ id: '123' }),
            },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(FeedbackDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
