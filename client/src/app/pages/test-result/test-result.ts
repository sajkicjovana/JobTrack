import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  Router
} from '@angular/router';

import { TestAttempts } from '../../services/test-attempts';

@Component({
  selector: 'app-test-result',
  imports: [],
  templateUrl: './test-result.html',
  styleUrl: './test-result.scss',
})
export class TestResult implements OnInit {

  result: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private testAttemptsService: TestAttempts
  ) {}

  ngOnInit() {
    const attemptId =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    this.testAttemptsService
      .getResult(attemptId)
      .subscribe({
        next: (data) => {
          this.result = data;
        },
        error: (error) => {
          console.error(
            'Error loading result:',
            error
          );
        }
      });
  }

  getPercentage(): number {
    if (
      !this.result ||
      this.result.totalQuestions === 0
    ) {
      return 0;
    }

    return Math.round(
      this.result.correctAnswers /
      this.result.totalQuestions *
      100
    );
  }

  backToPreparation() {
    this.router.navigate([
      '/preparation'
    ]);
  }
}