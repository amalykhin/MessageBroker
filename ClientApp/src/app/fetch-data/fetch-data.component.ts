import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from "rxjs";
import {map, toArray} from "rxjs/operators";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css']
})
export class FetchDataComponent {
  stories: NewsStory[] = [];
  tags: string[] = [];
  clientId: number;
  isLoading: Observable<boolean> = of(false);

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    setInterval(() => {
      for (let tag of this.tags) {
        this.fetchNews(tag).subscribe(result => this.stories = [...result, ...this.stories].sort());
      }
    }, 1000);
  }

  subscriptionHandler(input) {
    this.subscribe(input.value);
    input.value = '';
  }

  subscribe(tag: string) {
    this.http.get<number>(`${this.baseUrl}subscribe/${tag}`).subscribe(result => {
      if (this.tags.includes(tag)) {
        return;
      }
      this.tags.push(tag);
      this.clientId = result;

      this.fetchNews(tag).subscribe(result => this.stories = [...this.stories, ...result].sort());
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
