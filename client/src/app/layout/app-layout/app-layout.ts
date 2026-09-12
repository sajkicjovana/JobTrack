import {
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';

import { Auth } from '../../services/auth';

import {
  Notifications,
  TestPublishedNotification
} from '../../services/notifications';


@Component({
  selector: 'app-app-layout',

  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],

  templateUrl: './app-layout.html',
  styleUrl: './app-layout.scss',
})
export class AppLayout
  implements OnInit, OnDestroy {

  notification:
    TestPublishedNotification | null = null;

  private notificationTimeout:
    ReturnType<typeof setTimeout> | null = null;


  constructor(
    private authService: Auth,
    private router: Router,
    private notificationsService:
      Notifications
  ) {}


  ngOnInit() {

    if (!this.isAdmin()) {

      this.notificationsService
        .startConnection(
          notification => {
            this.showNotification(
              notification
            );
          }
        );
    }
  }


  ngOnDestroy() {

    this.notificationsService
      .stopConnection();

    if (this.notificationTimeout) {
      clearTimeout(
        this.notificationTimeout
      );
    }
  }


  isAdmin(): boolean {
    return this.authService.isAdmin();
  }


  getEmail(): string {
    return this.authService.getEmail();
  }


  showNotification(
    notification:
      TestPublishedNotification
  ) {

    this.notification =
      notification;


    if (this.notificationTimeout) {
      clearTimeout(
        this.notificationTimeout
      );
    }


    this.notificationTimeout =
      setTimeout(() => {

        this.notification = null;

      }, 5000);
  }


  closeNotification() {

    this.notification = null;

    if (this.notificationTimeout) {

      clearTimeout(
        this.notificationTimeout
      );

      this.notificationTimeout = null;
    }
  }

  goToPreparation() {

    if (!this.notification) {
      return;
    }

    this.router.navigate(
      ['/preparation'],
      {
        queryParams: {
          technology:
            this.notification.technologyName,

          testId:
            this.notification.id
        }
      }
    );

    this.closeNotification();
  }

  async logout() {

    await this.notificationsService
      .stopConnection();

    this.authService.logout();

    this.router.navigate([
      '/login'
    ]);
  }
}