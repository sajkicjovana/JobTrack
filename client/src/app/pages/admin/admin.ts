import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  Technologies,
  Technology
} from '../../services/technologies';

import {
  TopicsService,
  Topic
} from '../../services/topics';

import { Questions } from '../../services/questions';
import { Tests } from '../../services/tests';

@Component({
  selector: 'app-admin',
  imports: [FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin implements OnInit {

  activeTab:
    'technologies' |
    'topics' |
    'questions' |
    'tests' = 'technologies';

  technologies: Technology[] = [];
  topics: Topic[] = [];
  questions: any[] = [];
  tests: any[] = [];

  technologyName = '';
  editingTechnologyId: number | null = null;

  topicName = '';
  topicTechnologyId: number | null = null;
  editingTopicId: number | null = null;
  topicContent = '';

  topicFilterTechnologyId: number | null = null;
  questionFilterTechnologyId: number | null = null;
  testFilterTechnologyId: number | null = null;

  questionText = '';

  questionTechnologyId: number | null = null;
  questionTopicId: number | null = null;

  editingQuestionId: number | null = null;

  answers: {
    text: string;
    isCorrect: boolean;
  }[] = [
    {
      text: '',
      isCorrect: true
    },
    {
      text: '',
      isCorrect: false
    }
  ];

  testTitle = '';
  testDescription = '';

  testTechnologyId: number | null = null;

  selectedQuestionIds: number[] = [];

  editingTestId: number | null = null;

  private technologiesService = inject(Technologies);
  private topicsService = inject(TopicsService);
  private questionsService = inject(Questions);
  private testsService = inject(Tests);


  ngOnInit() {
    this.loadAll();
  }


  loadAll() {
    this.loadTechnologies();
    this.loadTopics();
    this.loadQuestions();
    this.loadTests();
  }

  setTab(
    tab:
      'technologies' |
      'topics' |
      'questions' |
      'tests'
  ) {
    this.activeTab = tab;
  }


  loadTechnologies() {
    this.technologiesService.getAll().subscribe({
      next: (data) => {
        this.technologies = data;
      },
      error: (error) => {
        console.error(
          'Error loading technologies:',
          error
        );
      }
    });
  }


  saveTechnology() {
    const name = this.technologyName.trim();

    if (!name) {
       alert('Enter a technology name.');
      return;
    }

    if (this.editingTechnologyId) {

      this.technologiesService
        .update(
          this.editingTechnologyId,
          name
        )
        .subscribe({
          next: () => {
            this.resetTechnologyForm();
            this.loadTechnologies();
          },
          error: (error) => {
            console.error(
              'Error updating technology:',
              error.error
            );
          }
        });

    } else {

      this.technologiesService
        .create(name)
        .subscribe({
          next: () => {
            this.resetTechnologyForm();
            this.loadTechnologies();
          },
          error: (error) => {
            console.error(
              'Error creating technology:',
              error.error
            );
          }
        });

    }
  }


  editTechnology(technology: Technology) {
    this.editingTechnologyId = technology.id;
    this.technologyName = technology.name;

    this.scrollToForm();
  }


  deleteTechnology(id: number) {
    if (
      !confirm(
        'Are you sure you want to delete this technology?'
      )
    ) {
      return;
    }

    this.technologiesService.delete(id).subscribe({
      next: () => {
        this.loadAll();
      },
      error: (error) => {
        alert(
          error.error ||
          'Technology cannot be deleted.'
        );
      }
    });
  }


  resetTechnologyForm() {
    this.technologyName = '';
    this.editingTechnologyId = null;
  }


  loadTopics() {
    this.topicsService.getAll().subscribe({
      next: (data) => {
        this.topics = data;
      },
      error: (error) => {
        console.error(
          'Error loading topics:',
          error
        );
      }
    });
  }


  saveTopic() {
    if (this.topicTechnologyId == null) {
      alert('Select a technology.');
      return;
    }

    if (!this.topicName.trim()) {
      alert('Enter a topic name.');
      return;
    }

    if (!this.topicContent.trim()) {
      alert('Enter theory content.');
      return;
    }

    const data = {
      name: this.topicName.trim(),
      content: this.topicContent.trim(),
      technologyId:
        Number(this.topicTechnologyId)
    };

    if (this.editingTopicId) {

      this.topicsService
        .update(
          this.editingTopicId,
          data
        )
        .subscribe({
          next: () => {
            this.resetTopicForm();
            this.loadTopics();
          },
          error: (error) => {
            console.error(
              'Error updating topic:',
              error.error
            );
          }
        });

    } else {

      this.topicsService
        .create(data)
        .subscribe({
          next: () => {
            this.resetTopicForm();
            this.loadTopics();
          },
         error: (error) => {
          alert(
            error.error ||
            'Topic could not be created.'
          );
        }
        });

    }
  }


  editTopic(topic: Topic) {
    this.editingTopicId = topic.id;
    this.topicName = topic.name;
    this.topicContent = topic.content ?? '';
    this.topicTechnologyId =
      topic.technologyId;

    this.scrollToForm();
  }

  deleteTopic(id: number) {
    if (
      !confirm(
        'Are you sure you want to delete this topic?'
      )
    ) {
      return;
    }

    this.topicsService.delete(id).subscribe({
      next: () => {
        this.loadAll();
      },
      error: (error) => {
        alert(
          error.error ||
          'Topic cannot be deleted.'
        );
      }
    });
  }


  resetTopicForm() {
    this.topicName = '';
    this.topicContent = '';
    this.topicTechnologyId = null;
    this.editingTopicId = null;
  }


  loadQuestions() {
    this.questionsService.getAll().subscribe({
      next: (data) => {
        this.questions = data;
      },
      error: (error) => {
        console.error(
          'Error loading questions:',
          error
        );
      }
    });
  }


  getQuestionTopics(): Topic[] {
    if (this.questionTechnologyId == null) {
      return [];
    }

    return this.topics.filter(
      topic =>
        topic.technologyId ===
        Number(this.questionTechnologyId)
    );
  }


  onQuestionTechnologyChange() {
    this.questionTopicId = null;
  }


  addAnswer() {
    this.answers.push({
      text: '',
      isCorrect: false
    });
  }


  removeAnswer(index: number) {
    if (this.answers.length <= 2) {
      return;
    }

    this.answers.splice(index, 1);

    if (
      !this.answers.some(
        answer => answer.isCorrect
      )
    ) {
      this.answers[0].isCorrect = true;
    }
  }


  setCorrectAnswer(index: number) {
    this.answers.forEach(
      (answer, answerIndex) => {
        answer.isCorrect =
          answerIndex === index;
      }
    );
  }


  saveQuestion() {
    if (this.questionTechnologyId == null) {
      alert('Select a technology.');
      return;
    }

    if (this.questionTopicId == null) {
      alert('Select a topic.');
      return;
    }

    if (!this.questionText.trim()) {
      alert('Enter the question text.');
      return;
    }

    const validAnswers =
      this.answers.filter(
        answer => answer.text.trim()
      );

    if (validAnswers.length < 2) {
      alert(
        'Question must have at least two answers.'
      );
      return;
    }

    if (
      validAnswers.filter(
        answer => answer.isCorrect
      ).length !== 1
    ) {
      alert(
        'Question must have exactly one correct answer.'
      );
      return;
    }

    const data = {
      text: this.questionText.trim(),

      topicId:
        Number(this.questionTopicId),

      answers:
        validAnswers.map(answer => ({
          text: answer.text.trim(),
          isCorrect: answer.isCorrect
        }))
    };


    if (this.editingQuestionId) {

      this.questionsService
        .update(
          this.editingQuestionId,
          data
        )
        .subscribe({
          next: () => {
            this.resetQuestionForm();
            this.loadAll();
          },
          error: (error) => {
            console.error(
              'Error updating question:',
              error.error
            );
          }
        });

    } else {

      this.questionsService
        .create(data)
        .subscribe({
          next: () => {
            this.resetQuestionForm();
            this.loadAll();
          },
          error: (error) => {
            console.error(
              'Error creating question:',
              error.error
            );
          }
        });

    }
  }


  editQuestion(question: any) {
    this.editingQuestionId =
      question.id;

    this.questionText =
      question.text;

    this.questionTechnologyId =
      question.technologyId;

    this.questionTopicId =
      question.topicId;

    this.answers =
      question.answers.map(
        (answer: any) => ({
          text: answer.text,
          isCorrect: answer.isCorrect
        })
      );
    this.scrollToForm();
  }


  deleteQuestion(id: number) {
    if (
      !confirm(
        'Are you sure you want to delete this question?'
      )
    ) {
      return;
    }

    this.questionsService.delete(id).subscribe({
      next: () => {
        this.loadAll();
      },
      error: (error) => {
        alert(
          error.error ||
          'Question cannot be deleted.'
        );
      }
    });
  }


  resetQuestionForm() {
    this.questionText = '';

    this.questionTechnologyId = null;
    this.questionTopicId = null;

    this.editingQuestionId = null;

    this.answers = [
      {
        text: '',
        isCorrect: true
      },
      {
        text: '',
        isCorrect: false
      }
    ];
  }

  loadTests() {
    this.testsService.getAllAdmin().subscribe({
      next: (data) => {
        this.tests = data;
      },
      error: (error) => {
        console.error(
          'Error loading tests:',
          error
        );
      }
    });
  }


  getQuestionsForTest() {
    if (this.testTechnologyId == null) {
      return [];
    }

    return this.questions.filter(
      question =>
        question.technologyId ===
        Number(this.testTechnologyId)
    );
  }


  onTestTechnologyChange() {
    this.selectedQuestionIds = [];
  }


  toggleTestQuestion(id: number) {
    if (
      this.selectedQuestionIds.includes(id)
    ) {
      this.selectedQuestionIds =
        this.selectedQuestionIds.filter(
          questionId =>
            questionId !== id
        );
    } else {
      this.selectedQuestionIds.push(id);
    }
  }


  isQuestionSelected(id: number): boolean {
    return this.selectedQuestionIds.includes(id);
  }


  saveTest() {
    if (!this.testTitle.trim()) {
      alert('Enter a test title.');
      return;
    }

    if (this.testTechnologyId == null) {
      alert('Select a technology.');
      return;
    }

    if (
      this.selectedQuestionIds.length === 0
    ) {
      alert(
        'Select at least one question.'
      );
      return;
    }

    const data = {
      title:
        this.testTitle.trim(),

      description:
        this.testDescription.trim() || null,

      technologyId:
        Number(this.testTechnologyId),

      questionIds:
        this.selectedQuestionIds
    };


    if (this.editingTestId) {

      this.testsService
        .update(
          this.editingTestId,
          data
        )
        .subscribe({
          next: () => {
            this.resetTestForm();
            this.loadTests();
          },
          error: (error) => {
            alert(
              error.error ||
              'Error updating test.'
            );
          }
        });

    } else {

      this.testsService
        .create(data)
        .subscribe({
          next: () => {
            this.resetTestForm();
            this.loadTests();
          },
          error: (error) => {
            console.error(
              'Error creating test:',
              error.error
            );
          }
        });

    }
  }


  editTest(test: any) {
    if (test.isPublished) {
      alert(
        'Unpublish the test before editing it.'
      );
      return;
    }

    this.testsService
      .getAdminById(test.id)
      .subscribe({
        next: (data) => {
          this.editingTestId = data.id;

          this.testTitle =
            data.title;

          this.testDescription =
            data.description ?? '';

          this.testTechnologyId =
            data.technologyId;

          this.selectedQuestionIds =
            data.questions.map(
              (question: any) =>
                question.id
            );
          this.scrollToForm();
        },
        error: (error) => {
          console.error(
            'Error loading test:',
            error
          );
        }
      });
  }


  publishTest(id: number) {
    this.testsService.publish(id).subscribe({
      next: () => {
        this.loadTests();
      },
      error: (error) => {
        alert(
          error.error ||
          'Test could not be published.'
        );
      }
    });
  }


  unpublishTest(id: number) {
    this.testsService.unpublish(id).subscribe({
      next: () => {
        this.loadTests();
      },
      error: (error) => {
        console.error(
          'Error unpublishing test:',
          error
        );
      }
    });
  }


  deleteTest(id: number) {
    if (
      !confirm(
        'Are you sure you want to delete this test?'
      )
    ) {
      return;
    }

    this.testsService.delete(id).subscribe({
      next: () => {
        this.loadTests();
      },
      error: (error) => {
        alert(
          error.error ||
          'Test cannot be deleted.'
        );
      }
    });
  }


    resetTestForm() {
      this.testTitle = '';
      this.testDescription = '';
      this.testTechnologyId = null;

      this.selectedQuestionIds = [];

      this.editingTestId = null;
    }
    scrollToForm() {
      setTimeout(() => {
        window.scrollTo({
          top: 0,
          behavior: 'smooth'
        });
      }, 0);
    }
    get filteredAdminTopics(): Topic[] {

    if (this.topicFilterTechnologyId == null) {
      return this.topics;
    }

    return this.topics.filter(
      topic =>
        topic.technologyId ===
        Number(this.topicFilterTechnologyId)
    );
  }


  get filteredAdminQuestions(): any[] {

    if (this.questionFilterTechnologyId == null) {
      return this.questions;
    }

    return this.questions.filter(
      question =>
        question.technologyId ===
        Number(this.questionFilterTechnologyId)
    );
  }


  get filteredAdminTests(): any[] {

    if (this.testFilterTechnologyId == null) {
      return this.tests;
    }

    return this.tests.filter(
      test =>
        test.technologyId ===
        Number(this.testFilterTechnologyId)
    );
  }
}