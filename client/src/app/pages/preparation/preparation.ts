import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs';
import { Notifications} from '../../services/notifications';
import { FormsModule } from '@angular/forms';
import { Tests } from '../../services/tests';
import { TestAttempts} from '../../services/test-attempts';
import {TopicsService, Topic} from '../../services/topics';



@Component({
  selector: 'app-preparation',
  imports: [FormsModule],
  templateUrl: './preparation.html',
  styleUrl: './preparation.scss',
})
export class Preparation implements OnInit, OnDestroy {

  tests: any[] = [];
  results: any[] = [];

  topics: Topic[] = [];

  searchTerm = '';
  selectedTechnology = 'All';

  highlightedTestId: number | null = null;

  private notificationSubscription: Subscription | null = null;
  private publishedSubscription:Subscription | null = null;

    constructor(
      private testsService: Tests,
      private testAttemptsService: TestAttempts,
      private topicsService: TopicsService,
      private router: Router,
      private route: ActivatedRoute,
      private notificationsService: Notifications
    ) {}


  ngOnInit() {
    this.notificationSubscription =
      this.notificationsService
        .testUnpublished$
        .subscribe(testId => {

          this.tests =
            this.tests.filter(
              test =>
                test.id !== testId
            );

        });
     
    this.publishedSubscription =
      this.notificationsService
        .testPublished$
        .subscribe(() => {

          this.loadTests();

        });    
    this.loadTopics();
    this.loadResults();


    this.route.queryParamMap
      .subscribe(params => {

        const technology =
          params.get('technology');

        const testIdParam =
          params.get('testId');


        if (technology) {
          this.selectedTechnology =
            technology;
        }


        if (testIdParam) {
          this.highlightedTestId =
            Number(testIdParam);
        } else {
          this.highlightedTestId =
            null;
        }

        this.loadTests(
          this.highlightedTestId
        );

      });
  }


  loadTests(
    scrollToTestId:
      number | null = null
  ) {

    this.testsService
      .getPublished()
      .subscribe({

        next: (data) => {

          this.tests = data;


          if (scrollToTestId != null) {

            setTimeout(() => {

              const element =
                document.getElementById(
                  `test-${scrollToTestId}`
                );


              if (element) {

                element.scrollIntoView({
                  behavior: 'smooth',
                  block: 'center'
                });

              }

            }, 100);

          }

        },

        error: (error) => {
          console.error(
            'Error loading tests:',
            error
          );
        }

      });
  }


  loadTopics() {

    this.topicsService
      .getAll()
      .subscribe({

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


  loadResults() {

    this.testAttemptsService
      .getMyResults()
      .subscribe({

        next: (data) => {
          this.results = data;
        },

        error: (error) => {
          console.error(
            'Error loading results:',
            error
          );
        }

      });
  }


  get technologies(): string[] {

    const names = [

      ...this.tests.map(
        test => test.technologyName
      ),

      ...this.topics.map(
        topic => topic.technologyName
      )

    ].filter(name => name);


    return [
      'All',
      ...Array.from(
        new Set(names)
      )
    ];
  }


  get filteredTopics(): Topic[] {

    if (
      this.selectedTechnology === 'All'
    ) {
      return this.topics;
    }


    return this.topics.filter(
      topic =>
        topic.technologyName ===
        this.selectedTechnology
    );
  }


  get filteredTests(): any[] {

    const search =
      this.searchTerm
        .trim()
        .toLowerCase();


    return this.tests.filter(test => {

      const matchesTechnology =
        this.selectedTechnology === 'All' ||
        test.technologyName ===
          this.selectedTechnology;


      const matchesSearch =
        search === '' ||
        test.title
          ?.toLowerCase()
          .includes(search) ||
        test.description
          ?.toLowerCase()
          .includes(search) ||
        test.technologyName
          ?.toLowerCase()
          .includes(search);


      return (
        matchesTechnology &&
        matchesSearch
      );
    });
  }


  get groupedTests(): {
    technologyName: string;
    tests: any[];
  }[] {

    const groups =
      new Map<string, any[]>();


    this.filteredTests.forEach(test => {

      const technology =
        test.technologyName ?? 'Other';


      if (!groups.has(technology)) {
        groups.set(
          technology,
          []
        );
      }


      groups
        .get(technology)!
        .push(test);
    });


    return Array
      .from(groups.entries())
      .map(
        ([technologyName, tests]) => ({
          technologyName,
          tests
        })
      );
  }


  selectTechnology(
    technology: string
  ) {

    this.selectedTechnology =
      technology;

    this.highlightedTestId =
      null;
  }


  openTopic(id: number) {

    this.router.navigate([
      '/preparation/topics',
      id
    ]);
  }


  startTest(testId: number) {

    this.router.navigate([
      '/tests',
      testId
    ]);
  }


  formatDate(date: string): string {

    if (!date) {
      return '';
    }

    return new Date(date)
      .toLocaleDateString('en-GB');
  }


  formatPercentage(
    value: number
  ): string {

    return `${Math.round(value)}%`;
  }

  ngOnDestroy(): void {

    this.notificationSubscription
      ?.unsubscribe();

    this.publishedSubscription
      ?.unsubscribe();
  }
}