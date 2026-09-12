import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  Router
} from '@angular/router';

import { TestAttempts } from '../../services/test-attempts';

@Component({
  selector: 'app-take-test',
  imports: [],
  templateUrl: './take-test.html',
  styleUrl: './take-test.scss',
})
export class TakeTest implements OnInit {

  test: any = null;

  attemptId: number | null = null;

  currentQuestionIndex = 0;

  selectedAnswers:
    Record<number, number> = {};

  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private testAttemptsService: TestAttempts
  ) {}

  ngOnInit() {
    const testId =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    this.testAttemptsService
      .start(testId)
      .subscribe({
        next: (data) => {
          this.attemptId =
            data.attemptId;

          this.test = data;

          this.loading = false;
        },
        error: (error) => {
          console.error(
            'Error starting test:',
            error
          );

          this.loading = false;
        }
      });
  }

  get currentQuestion() {
    return this.test?.questions[
      this.currentQuestionIndex
    ];
  }

  selectAnswer(
    questionId: number,
    answerId: number
  ) {
    this.selectedAnswers[questionId] =
      answerId;
  }

  isSelected(
    questionId: number,
    answerId: number
  ): boolean {
    return (
      this.selectedAnswers[questionId] ===
      answerId
    );
  }

  previousQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
    }
  }

  nextQuestion() {
    if (
      this.currentQuestionIndex <
      this.test.questions.length - 1
    ) {
      this.currentQuestionIndex++;
    }
  }

  submitTest() {
    if (this.attemptId == null) {
      return;
    }

    const unanswered =
      this.test.questions.filter(
        (question: any) =>
          !this.selectedAnswers[question.id]
      );

    if (unanswered.length > 0) {
      const confirmed = confirm(
        `You have ${unanswered.length} unanswered question(s). Submit anyway?`
      );

      if (!confirmed) {
        return;
      }
    }

    const answers =
      Object.entries(this.selectedAnswers)
        .map(
          ([questionId, answerOptionId]) => ({
            questionId:
              Number(questionId),

            answerOptionId:
              Number(answerOptionId)
          })
        );

    this.testAttemptsService
      .submit(
        this.attemptId,
        answers
      )
      .subscribe({
        next: () => {
          this.router.navigate([
            '/test-result',
            this.attemptId
          ]);
        },
        error: (error) => {
          console.error(
            'Error submitting test:',
            error.error
          );
        }
      });
  }
}