import { Injectable } from '@angular/core';

import {
  Subject
} from 'rxjs';

import * as signalR
  from '@microsoft/signalr';


export interface TestPublishedNotification {
  id: number;
  title: string;
  technologyId: number;
  technologyName: string;
}


@Injectable({
  providedIn: 'root'
})
export class Notifications {

  private hubConnection:
    signalR.HubConnection | null = null;


  private testUnpublishedSubject =
    new Subject<number>();


  testUnpublished$ =
    this.testUnpublishedSubject.asObservable();

  private testPublishedSubject =
    new Subject<TestPublishedNotification>();

  testPublished$ =
    this.testPublishedSubject.asObservable();

  async startConnection(
    onTestPublished:
      (
        notification:
          TestPublishedNotification
      ) => void
  ) {

    const token =
      localStorage.getItem('token');


    if (!token) {
      return;
    }


    if (
      this.hubConnection?.state ===
      signalR.HubConnectionState.Connected
    ) {
      return;
    }


    this.hubConnection =
      new signalR.HubConnectionBuilder()
        .withUrl(
          'http://localhost:5254/hubs/notifications',
          {
            accessTokenFactory: () =>
              localStorage.getItem(
                'token'
              ) ?? ''
          }
        )
        .withAutomaticReconnect()
        .build();


    this.hubConnection.off(
      'TestPublished'
    );

    this.hubConnection.off(
      'TestUnpublished'
    );


    this.hubConnection.on(
      'TestPublished',
      (
        notification:
          TestPublishedNotification
      ) => {

        this.testPublishedSubject
          .next(notification);

        onTestPublished(
          notification
        );

      }
    );


    this.hubConnection.on(
      'TestUnpublished',
      (
        data: {
          id: number;
        }
      ) => {

        this.testUnpublishedSubject
          .next(data.id);

      }
    );


    try {

      await this.hubConnection.start();

      console.log(
        'SignalR connected.'
      );

    } catch (error) {

      console.error(
        'SignalR connection error:',
        error
      );

    }
  }


  async stopConnection() {

    if (!this.hubConnection) {
      return;
    }


    try {

      await this.hubConnection.stop();

    } catch (error) {

      console.error(
        'Error stopping SignalR:',
        error
      );

    }


    this.hubConnection = null;
  }
}