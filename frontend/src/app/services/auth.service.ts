import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { jwtDecode } from 'jwt-decode';
import { isNil } from "ng-zorro-antd/core/util";
import { map, Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { AuthResponseModel } from "../shared/models/auth-response-model";
import { LoginModel } from "../shared/models/login-model";
import { RegisterModel } from "../shared/models/register-model";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'SimpliasTaskDemo_Token';
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  public login(viewModel: LoginModel): Observable<AuthResponseModel> {
    return this.http.post<AuthResponseModel>(`${this.apiUrl}accounts/login`, viewModel)
      .pipe(
        map((res) => {
          if (res.result) {
            localStorage.setItem(this.tokenKey, res.token);
          }

          return res;
        })
      )
  }

  public logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  public register(viewModel: RegisterModel): Observable<AuthResponseModel> {
    return this.http.post<AuthResponseModel>(`${this.apiUrl}accounts/register`, viewModel)
      .pipe(
        map((res) => {
          if (res.result) {
            localStorage.setItem(this.tokenKey, res.token);
          }

          return res;
        })
      )
  }

  public isLoggedIn(): boolean {
    const token = this.retrieveToken();
    if (token === null) {
      return false;
    }

    return this.isTokenValid(token);
  }

  public retrieveToken(): string | null {
    return localStorage.getItem(this.tokenKey) || null;
  }

  private isTokenValid(token: string): boolean {
    if (isNil(token)) {
      return false;
    }

    const decodedToken = jwtDecode(token);
    const expiration = decodedToken.exp ?? 0

    if (Date.now() < expiration + 1000) {
      this.logout();
      return false;
    }

    return true;
  }
}
