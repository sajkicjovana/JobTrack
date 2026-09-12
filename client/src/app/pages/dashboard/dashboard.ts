import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { forkJoin } from 'rxjs';

import {
  DashboardService,
  DashboardInterview,
  DashboardTestResult
} from '../../services/dashboard';


interface TechnologyProgress {
  technologyName: string;
  score: number;
  testsCompleted: number;
}


interface ReadinessTechnology {
  name: string;
  score: number;
}


interface ApplicationReadiness {
  applicationId: number;
  companyName: string;
  position: string;
  score: number | null;
  technologies: ReadinessTechnology[];
}


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {

  private dashboardService =
    inject(DashboardService);


  applicationsCount = 0;
  upcomingInterviewsCount = 0;
  testsCompleted = 0;
  averageScore = 0;


  upcomingInterviews:
    DashboardInterview[] = [];

  recentResults:
    DashboardTestResult[] = [];


  technologyProgress:
    TechnologyProgress[] = [];

  applicationReadiness:
    ApplicationReadiness[] = [];


  loading = true;
  errorMessage = '';


  ngOnInit(): void {
    this.loadDashboard();
  }


  loadDashboard(): void {

    this.loading = true;
    this.errorMessage = '';


    forkJoin({

      applications:
        this.dashboardService
          .getApplications(),

      interviews:
        this.dashboardService
          .getInterviews(),

      results:
        this.dashboardService
          .getResults()

    }).subscribe({

      next: ({
        applications,
        interviews,
        results
      }) => {

        this.applicationsCount =
          applications.length;


        const now = new Date();


        this.upcomingInterviews =
          interviews
            .filter(interview =>
              new Date(
                interview.interviewDate
              ) >= now &&
              !interview.outcome
            )
            .sort(
              (a, b) =>
                new Date(
                  a.interviewDate
                ).getTime()
                -
                new Date(
                  b.interviewDate
                ).getTime()
            );


        this.upcomingInterviewsCount =
          this.upcomingInterviews.length;


        this.testsCompleted =
          results.length;


        if (results.length > 0) {

          const total =
            results.reduce(
              (sum, result) =>
                sum + result.percentage,
              0
            );

          this.averageScore =
            Math.round(
              (total / results.length) * 10
            ) / 10;

        } else {

          this.averageScore = 0;
        }


        this.recentResults =
          [...results]
            .sort(
              (a, b) =>
                new Date(
                  b.completedAt
                ).getTime()
                -
                new Date(
                  a.completedAt
                ).getTime()
            )
            .slice(0, 3);


        this.calculateTechnologyProgress(
          applications,
          results
        );


        this.calculateApplicationReadiness(
          applications
        );


        this.loading = false;
      },


      error: () => {

        this.errorMessage =
          'Unable to load dashboard data.';

        this.loading = false;
      }

    });
  }


  calculateTechnologyProgress(
    applications: any[],
    results: DashboardTestResult[]
  ): void {

    const technologyNames =
      new Set<string>();


    applications.forEach(application => {

      application.technologies?.forEach(
        (technology: any) => {

          technologyNames.add(
            technology.name
          );

        }
      );

    });


    results.forEach(result => {

      technologyNames.add(
        result.technologyName
      );

    });


    this.technologyProgress =
      Array.from(technologyNames)
        .map(technologyName => {

          const technologyResults =
            results.filter(
              result =>
                result.technologyName
                  .toLowerCase()
                ===
                technologyName
                  .toLowerCase()
            );


          const bestResults =
            new Map<number, number>();


          technologyResults.forEach(result => {

            const currentBest =
              bestResults.get(
                result.testId
              );


            if (
              currentBest === undefined ||
              result.percentage > currentBest
            ) {

              bestResults.set(
                result.testId,
                result.percentage
              );

            }

          });


          const scores =
            Array.from(
              bestResults.values()
            );


          let score = 0;


          if (scores.length > 0) {

            const total =
              scores.reduce(
                (sum, value) =>
                  sum + value,
                0
              );

          score =
            Math.round(
              (total / scores.length) * 10
            ) / 10;
          }


          return {
            technologyName,
            score,
            testsCompleted:
              bestResults.size
          };

        })
        .sort(
          (a, b) =>
            b.score - a.score
        );
  }


  calculateApplicationReadiness(
    applications: any[]
  ): void {

    this.applicationReadiness =
      applications.map(application => {

        const technologies =
          application.technologies ?? [];


        if (technologies.length === 0) {

          return {
            applicationId:
              application.id,

            companyName:
              application.companyName,

            position:
              application.position,

            score: null,

            technologies: []
          };

        }


        const readinessTechnologies:
          ReadinessTechnology[] =
          technologies.map(
            (technology: any) => ({

              name:
                technology.name,

              score:
                this.getTechnologyScore(
                  technology.name
                )

            })
          );


        const total =
          readinessTechnologies
            .reduce(
              (sum, technology) =>
                sum + technology.score,
              0
            );


        const score =
          Math.round(
            (
              total /
              readinessTechnologies.length
            ) * 10
          ) / 10;


        return {

          applicationId:
            application.id,

          companyName:
            application.companyName,

          position:
            application.position,

          score,

          technologies:
            readinessTechnologies
        };

      });
  }


  getTechnologyScore(
    technologyName: string
  ): number {

    const progress =
      this.technologyProgress.find(
        technology =>
          technology.technologyName
            .toLowerCase()
          ===
          technologyName
            .toLowerCase()
      );


    return progress?.score ?? 0;
  }

    formatPercentage(
    value: number
  ): string {

    const rounded =
      Math.round(value * 10) / 10;

    if (Number.isInteger(rounded)) {
      return `${rounded}%`;
    }

    return `${rounded.toFixed(1)}%`;
  }
}