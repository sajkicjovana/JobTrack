import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Interviews } from '../../services/interviews';
import { JobApplications } from '../../services/job-applications';

@Component({
  selector: 'app-interviews',
  imports: [FormsModule],
  templateUrl: './interviews.html',
  styleUrl: './interviews.scss',
})
export class InterviewsPage implements OnInit {

  interviews: any[] = [];
  applications: any[] = [];

  showForm = false;
  editingInterview: any = null;

  jobApplicationId: number | null = null;
  interviewDate = '';
  type = 'HR';
  contactPerson = '';
  outcome = '';
  notes = '';

  constructor(
    private interviewsService: Interviews,
    private jobApplicationsService: JobApplications
  ) {}

  ngOnInit() {
    this.loadInterviews();
    this.loadApplications();
  }

  loadInterviews() {
    this.interviewsService.getAll().subscribe({
      next: (data) => {
        this.interviews = data;
      },
      error: (error) => {
        console.error(
          'Error loading interviews:',
          error
        );
      }
    });
  }

  loadApplications() {
    this.jobApplicationsService.getAll().subscribe({
      next: (data) => {
        this.applications = data;
      },
      error: (error) => {
        console.error(
          'Error loading applications:',
          error
        );
      }
    });
  }

  openForm() {
    this.editingInterview = null;
    this.resetForm();
    this.showForm = true;
  }

  openEditForm(interview: any) {
    this.editingInterview = interview;

    this.jobApplicationId =
      interview.jobApplicationId;

    this.type =
      interview.type;

    this.contactPerson =
      interview.contactPerson ?? '';

    this.outcome =
      interview.outcome ?? '';

    this.notes =
      interview.notes ?? '';

    if (interview.interviewDate) {
  const date = new Date(interview.interviewDate);

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');

  this.interviewDate =
    `${year}-${month}-${day}T${hours}:${minutes}`;
    } else {
      this.interviewDate = '';
    }

    this.showForm = true;

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  closeForm() {
    this.showForm = false;
    this.editingInterview = null;
    this.resetForm();
  }

  onSubmit() {
    if (this.jobApplicationId == null) {
      return;
    }

    const data = {
      jobApplicationId:
        Number(this.jobApplicationId),

      interviewDate:
        new Date(
          this.interviewDate
        ).toISOString(),

      type:
        this.type,

      contactPerson:
        this.contactPerson || null,

      outcome:
        this.outcome || null,

      notes:
        this.notes || null
    };

    if (this.editingInterview) {
      this.interviewsService
        .update(
          this.editingInterview.id,
          data
        )
        .subscribe({
          next: () => {
            this.closeForm();
            this.loadInterviews();
          },
          error: (error) => {
            console.error(
              'Error updating interview:',
              error.error
            );
          }
        });
    } else {
      this.interviewsService
        .create(data)
        .subscribe({
          next: () => {
            this.closeForm();
            this.loadInterviews();
          },
          error: (error) => {
            console.error(
              'Error creating interview:',
              error.error
            );
          }
        });
    }
  }

  deleteInterview(id: number) {
    const confirmed = confirm(
      'Are you sure you want to delete this interview?'
    );

    if (!confirmed) {
      return;
    }

    this.interviewsService
      .delete(id)
      .subscribe({
        next: () => {
          this.loadInterviews();
        },
        error: (error) => {
          console.error(
            'Error deleting interview:',
            error.error
          );
        }
      });
  }

  resetForm() {
    this.jobApplicationId = null;
    this.interviewDate = '';
    this.type = 'HR';
    this.contactPerson = '';
    this.outcome = '';
    this.notes = '';
  }

  getTypeLabel(type: string): string {
    switch (type) {
      case 'HR':
        return 'HR Interview';

      case 'Technical':
        return 'Technical Interview';

      default:
        return 'Other';
    }
  }

  getOutcomeLabel(outcome: string | null): string {
    if (!outcome) {
      return 'Upcoming';
    }

    return outcome;
  }

  formatDateTime(date: string): string {
    if (!date) {
      return '';
    }

    return new Date(date).toLocaleString(
      'en-GB',
      {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }
    );
  }
}