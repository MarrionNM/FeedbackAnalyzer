import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiServiceService } from '../../services/api-service.service';
import { LoaderComponent } from '../../components/loader/loader.component';
import { Location } from '@angular/common';

@Component({
  selector: 'app-submit-feedback',
  standalone: true,
  imports: [CommonModule, FormsModule, LoaderComponent],
  templateUrl: './submit-feedback.component.html',
  styleUrls: ['./submit-feedback.component.scss'],
})
export class SubmitFeedbackComponent {
  text = '';
  email: string | null = null;
  loading = false;
  success = false;
  error: string | null = null;

  constructor(
    private apiService: ApiServiceService,
    private location: Location
  ) {}

  submit() {
    if (!this.text.trim()) {
      this.error = 'Feedback text is required.';
      return;
    }

    if (this.email && !this.validateEmail(this.email)) {
      this.error = 'Invalid email address.';
      return;
    }

    this.loading = true;
    this.success = false;
    this.error = '';

    this.apiService
      .submitFeedback({
        message: this.text,
        email: this.email,
      })
      .subscribe({
        next: () => {
          this.loading = false;
          this.success = true;
          this.text = '';
          this.email = null;
        },
        error: (err) => {
          this.loading = false;
          this.error = err || 'Failed to submit feedback.';
        },
      });
  }

  validateEmail(email: string): boolean {
    return /\S+@\S+\.\S+/.test(email);
  }

  goBack() {
    this.location.back();
  }
}
