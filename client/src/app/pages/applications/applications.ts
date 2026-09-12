import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { JobApplications } from '../../services/job-applications';
import {
  Technologies,
  Technology
} from '../../services/technologies';

@Component({
  selector: 'app-applications',
  imports: [FormsModule],
  templateUrl: './applications.html',
  styleUrl: './applications.scss',
})
export class Applications implements OnInit {

  applications: any[] = [];

  showForm = false;
  editingApplication: any = null;

  searchTerm = '';
  statusFilter = '';
  technologyFilterId: number | null = null;


  companyName = '';
  position = '';
  jobUrl = '';
  location = '';
  status = 'Saved';
  applicationDate = '';
  notes = '';

  positionSuggestions: string[] = [
    'Junior Software Developer',
    'Junior Frontend Developer',
    'Junior Backend Developer',
    'Junior .NET Developer',
    'Junior Angular Developer',
    'Junior React Developer',
    'Software Developer',
    'Software Engineer',
    'Frontend Developer',
    'Backend Developer',
    'Full Stack Developer',
    '.NET Developer',
    'Angular Developer',
    'React Developer',
    'Java Developer',
    'Python Developer',
    'Mobile Developer',
    'Android Developer',
    'QA Engineer',
    'Junior QA Engineer',
    'DevOps Engineer',
    'Data Analyst',
    'Data Engineer'
  ];

  filteredPositions: string[] = [];
  showPositionSuggestions = false;

  technologies: Technology[] = [];
  selectedTechnologyIds: number[] = [];

  constructor(
    private jobApplicationsService: JobApplications,
    private technologiesService: Technologies
  ) {}

  ngOnInit() {
    this.loadApplications();
    this.loadTechnologies();
  }

  loadApplications() {

    this.jobApplicationsService
      .getAll(
        this.searchTerm,
        this.statusFilter,
        this.technologyFilterId
      )
      .subscribe({

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
  applyFilters() {
    this.loadApplications();
  }


  clearFilters() {

    this.searchTerm = '';
    this.statusFilter = '';
    this.technologyFilterId = null;

    this.loadApplications();
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

  toggleTechnology(id: number) {
    if (this.isTechnologySelected(id)) {
      this.selectedTechnologyIds =
        this.selectedTechnologyIds.filter(
          technologyId => technologyId !== id
        );
    } else {
      this.selectedTechnologyIds.push(id);
    }
  }

  isTechnologySelected(id: number): boolean {
    return this.selectedTechnologyIds.includes(id);
  }

  openForm() {
    this.editingApplication = null;
    this.resetForm();
    this.showForm = true;
  }

  openEditForm(application: any) {
    this.editingApplication = application;

    this.companyName = application.companyName;
    this.position = application.position;
    this.jobUrl = application.jobUrl ?? '';
    this.location = application.location ?? '';
    this.status = application.status;

    this.selectedTechnologyIds =
      application.technologies
        ? application.technologies.map(
            (technology: Technology) => technology.id
          )
        : [];

    this.applicationDate = application.applicationDate
      ? application.applicationDate.split('T')[0]
      : '';

    this.notes = application.notes ?? '';

    this.showForm = true;

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  closeForm() {
    this.showForm = false;
    this.editingApplication = null;
    this.resetForm();
  }

  onSubmit() {
    const data = {
      companyName: this.companyName,
      position: this.position,
      jobUrl: this.jobUrl || null,
      location: this.location,
      status: this.status,

      applicationDate: this.applicationDate
        ? new Date(this.applicationDate).toISOString()
        : new Date().toISOString(),

      notes: this.notes,
      technologyIds: this.selectedTechnologyIds
    };

    if (this.editingApplication) {
      this.jobApplicationsService
        .update(this.editingApplication.id, data)
        .subscribe({
          next: () => {
            this.closeForm();
            this.loadApplications();
          },
          error: (error) => {
            console.error(
              'Error updating application:',
              error.error
            );
          }
        });
    } else {
      this.jobApplicationsService
        .create(data)
        .subscribe({
          next: () => {
            this.closeForm();
            this.loadApplications();
          },
          error: (error) => {
            console.error(
              'Error creating application:',
              error.error
            );
          }
        });
    }
  }

  deleteApplication(id: number) {
    const confirmed = confirm(
      'Are you sure you want to delete this application?'
    );

    if (!confirmed) {
      return;
    }

    this.jobApplicationsService.delete(id).subscribe({
      next: () => {
        this.loadApplications();
      },
      error: (error) => {
        console.error(
          'Error deleting application:',
          error.error
        );
      }
    });
  }

  resetForm() {
    this.companyName = '';
    this.position = '';
    this.jobUrl = '';
    this.location = '';
    this.status = 'Saved';
    this.applicationDate = '';
    this.notes = '';

    this.filteredPositions = [];
    this.showPositionSuggestions = false;
    this.selectedTechnologyIds = [];
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Saved':
        return 'Saved';

      case 'Applied':
        return 'Applied';

      case 'HRInterview':
        return 'HR Interview';

      case 'TechnicalInterview':
        return 'Technical Interview';

      case 'Offer':
        return 'Offer';

      case 'Rejected':
        return 'Rejected';

      case 'NoResponse':
        return 'No Response';

      default:
        return status;
    }
  }

  formatDate(date: string): string {
    if (!date) {
      return 'Not specified';
    }

    const parsedDate = new Date(date);

    return parsedDate.toLocaleDateString('en-GB');
  }

  onPositionInput() {
    const value = this.position.trim().toLowerCase();

    if (value.length === 0) {
      this.filteredPositions = [];
      this.showPositionSuggestions = false;
      return;
    }

    this.filteredPositions =
      this.positionSuggestions.filter(position =>
        position.toLowerCase().includes(value)
      );

    this.showPositionSuggestions = true;
  }

  selectPosition(position: string) {
    this.position = position;
    this.filteredPositions = [];
    this.showPositionSuggestions = false;
  }

  hidePositionSuggestions() {
    this.showPositionSuggestions = false;
  }
}