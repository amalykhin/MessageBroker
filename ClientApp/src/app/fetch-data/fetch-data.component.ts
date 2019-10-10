import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  private stories: NewsStory[];
  private tags: string[];
  private clientId: number;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    /*http.get<SubscriptionResponse>(baseUrl + 'subscribe/test').subscribe(result => {
      this.clientId = result.clientId;
      this.stories = result.news;
      console.log(`clientId=${this.clientId} result=${result.news[0]}`);
      http.get<NewsStory[]>(`${baseUrl}${this.clientId}/stories`).subscribe(result => {
        this.stories.concat(result);
      }, error => console.error(error));
    }, error => console.error(error));*/
    this.stories = [];
    this.tags = [];
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
      this.stories = [...this.stories, ...result.news];
      console.log(`clientId=${this.clientId} result=${result.news[0]}`);
      this.http.get<NewsStory[]>(`${this.baseUrl}${this.clientId}/stories`).subscribe(result => {
        this.stories = [...this.stories, ...result];
      }, error => console.error(error));
    }, error => console.error(error));
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
