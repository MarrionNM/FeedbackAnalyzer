import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SubmitFeedbackComponent } from './submit-feedback.component';
import { ApiServiceService } from '../../services/api-service.service';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { By } from '@angular/platform-browser';
import { LoaderComponent } from '../../components/loader/loader.component';

describe('SubmitFeedbackComponent', () => {
  let component: SubmitFeedbackComponent;
  let fixture: ComponentFixture<SubmitFeedbackComponent>;
  let apiMock: jasmine.SpyObj<ApiServiceService>;

  beforeEach(async () => {
    apiMock = jasmine.createSpyObj('ApiServiceService', ['submitFeedback']);

    await TestBed.configureTestingModule({
      imports: [SubmitFeedbackComponent, FormsModule, LoaderComponent],
      providers: [{ provide: ApiServiceService, useValue: apiMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(SubmitFeedbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should submit feedback when form is valid', () => {
    apiMock.submitFeedback.and.returnValue(of({}));

    const textarea = fixture.debugElement.query(
      By.css('textarea')
    ).nativeElement;
    textarea.value = 'This is feedback';
    textarea.dispatchEvent(new Event('input'));

    fixture.detectChanges();

    const button = fixture.debugElement.query(
      By.css('button[type="submit"]')
    ).nativeElement;
    button.click();

    expect(apiMock.submitFeedback).toHaveBeenCalledOnceWith({
      message: 'This is feedback',
      email: null,
    });
  });

  it('should show validation error if empty', () => {
    component.text = '';
    component.submit();
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.alert.error'));
    expect(error).toBeTruthy();
    expect(error.nativeElement.textContent).toContain(
      'Feedback text is required.'
    );
  });
});
