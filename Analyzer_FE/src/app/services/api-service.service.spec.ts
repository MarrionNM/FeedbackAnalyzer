import { TestBed } from '@angular/core/testing';
import { ApiServiceService } from './api-service.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ApiServiceService', () => {
  let service: ApiServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiServiceService],
    });
    service = TestBed.inject(ApiServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
