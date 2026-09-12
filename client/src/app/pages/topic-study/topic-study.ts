import {
  Component,
  OnInit,
  ViewEncapsulation
} from '@angular/core';

import {
  ActivatedRoute,
  RouterLink
} from '@angular/router';

import { marked } from 'marked';

import {
  TopicsService,
  Topic
} from '../../services/topics';


@Component({
  selector: 'app-topic-study',
  imports: [RouterLink],
  templateUrl: './topic-study.html',
  styleUrl: './topic-study.scss',
  encapsulation: ViewEncapsulation.None
})
export class TopicStudy implements OnInit {

  topic: Topic | null = null;

  contentHtml = '';

  loading = true;
  errorMessage = '';


  constructor(
    private route: ActivatedRoute,
    private topicsService: TopicsService
  ) {}


  ngOnInit() {

    const id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    this.topicsService
      .getById(id)
      .subscribe({

        next: (data) => {
          this.topic = data;

          this.contentHtml =
            marked.parse(
              data.content
            ) as string;

          this.loading = false;
        },

        error: () => {
          this.errorMessage =
            'Study material could not be loaded.';

          this.loading = false;
        }

      });
  }
}