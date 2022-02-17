import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  errorMessage !: string;
  loading: boolean = false;
  authForm!: FormGroup;
  token: string = this.route.snapshot.params['token'];
  helper: JwtHelperService;

  constructor(private formBuilder: FormBuilder, private authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.helper = new JwtHelperService();
    this.authForm = this.formBuilder.group({
      password: ['',[Validators.required]],
      passwordConfirmation: ['',[Validators.required]],
    });
   }

  ngOnInit(): void {
    //Check that token from the url is not expired
    if (this.helper.isTokenExpired(this.token)){
      this.router.navigateByUrl('/auth/sign-in');
    }
  }

  updatePassword(user: User){
    if (!this.authForm.valid) return;
    if (user.password !== user.passwordConfirmation) {
      this.errorMessage = "Passwords don't match!";
      return;
    }
    this.loading = true;
    const email = this.helper.decodeToken(this.token).email;
    user.email = email;
    return this.authService.updatePassword(user, this.token).subscribe(
      user => {
        this.loading = false;
        this.router.navigateByUrl('/auth/sign-in');
      }, error => {
        this.loading = false;
        this.errorMessage = "Something went wrong. Please try again!"
      }
    );
  }

}
