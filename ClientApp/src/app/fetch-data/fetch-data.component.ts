import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  private stories: NewsStory[];
  private clientId: number;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<SubscriptionResponse>(baseUrl + 'subscribe/test').subscribe(result => {
      this.clientId = result.clientId;
      this.stories = result.news;
      console.log(`clientId=${this.clientId} result=${result.news[0]}`);
      http.get<NewsStory[]>(`${baseUrl}${this.clientId}/stories`).subscribe(result => {
        this.stories.concat(result);
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
