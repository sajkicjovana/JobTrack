import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Register } from './pages/register/register';

import { AppLayout } from './layout/app-layout/app-layout';

import { Applications } from './pages/applications/applications';
import { InterviewsPage } from './pages/interviews/interviews';
import { Preparation } from './pages/preparation/preparation';
import { TakeTest } from './pages/take-test/take-test';
import { TestResult } from './pages/test-result/test-result';
import { Admin } from './pages/admin/admin';

import { authGuard } from './guards/auth-guard';
import { noAuthGuard } from './guards/no-auth-guard';
import { adminGuard } from './guards/admin-guard';
import { userGuard } from './guards/user-guard';

import { Dashboard } from './pages/dashboard/dashboard';

import { TopicStudy } from './pages/topic-study/topic-study';
export const routes: Routes = [

  {
    path: 'login',
    component: Login,
    canActivate: [noAuthGuard]
  },

  {
    path: 'register',
    component: Register,
    canActivate: [noAuthGuard]
  },

  {
    path: '',
    component: AppLayout,
    canActivate: [authGuard],

    children: [

      {
        path: 'applications',
        component: Applications,
        canActivate: [userGuard]
      },

      {
        path: 'interviews',
        component: InterviewsPage,
        canActivate: [userGuard]
      },

      {
        path: 'preparation',
        component: Preparation,
        canActivate: [userGuard]
      },
      {
        path: 'preparation/topics/:id',
        component: TopicStudy,
        canActivate: [userGuard]
      },

      {
        path: 'tests/:id',
        component: TakeTest,
        canActivate: [userGuard]
      },

      {
        path: 'test-result/:id',
        component: TestResult,
        canActivate: [userGuard]

      },
      {
        path: 'dashboard',
        component: Dashboard,
        canActivate: [userGuard]
      },

      {
        path: 'admin',
        component: Admin,
        canActivate: [adminGuard]
      },

      {
        path: '',
        redirectTo: 'applications',
        pathMatch: 'full'
      }

    ]
  },

  {
    path: '**',
    redirectTo: ''
  }

];