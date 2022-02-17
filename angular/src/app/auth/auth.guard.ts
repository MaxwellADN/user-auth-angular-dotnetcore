import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router){}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    //Get user from local storage
    let user = JSON.parse(localStorage.getItem("user")!);
    let jwtHelper = new JwtHelperService();
    //Check that user is not null, token is not null & token is not expired
    if (user != null && user.token != null && !jwtHelper.isTokenExpired(user.token)){
      //Get permitted roles on the current route the user is trying to access
      let roles = route.data['permittedRoles'] as Array<string>;
      // Check that permitted roles contains the user role
      if (user.role != null && roles.length > 0 && roles.includes(user.role.name)){
        return true; // This give access to the route the user trying to access
      }else{
        alert("You are allowed to access this page");
      }
    }
    this.router.navigateByUrl('/auth/sign-in')
    return false; // If none of this condition are met. This not grant access to the route for the user
  }

}
