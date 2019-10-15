import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {concat, forkJoin, Observable, of} from "rxjs";
import {map} from "rxjs/operators";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css']
})
export class FetchDataComponent {
  stories: NewsStory[];
  tags: string[];
  clientId: number;
  isLoading: Observable<boolean> = of(false);

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.stories = [];
    this.tags = [];

    setInterval(() => {
      for (let tag of this.tags) {
        this.fetchNews(tag).subscribe(result => this.stories = [...this.stories, ...result]);
      }
    }, 1000);
  }

  subscriptionHandler(input) {
    this.subscribe(input.value);
    input.value = '';
  }

  subscribe(tag: string) {
    this.http.get<SubscriptionResponse>(`${this.baseUrl}subscribe/${tag}`).subscribe(result => {
      if (this.tags.includes(tag)) {
        return;
      }
      this.tags.push(tag);
      this.clientId = result.clientId;

      this.fetchNews(tag).subscribe(result => this.stories = [...this.stories, ...result]);
      console.log(`clientId=${this.clientId} result=${result.news[0]}`);

    }, error => console.error(error));
  }

  fetchNews(tag: string): Observable<NewsStory[]> {
    return this.http.get<NewsStory[]>(`${this.baseUrl}${this.clientId}/stories`);
  }
}

interface NewsStory {
  tag: string;
  title: string;
  body: string;
  author: string;
  publishedDate: Date;
  modifiedDate: Date;
}

interface SubscriptionResponse {
  clientId: number;
  news: NewsStory[];
}
