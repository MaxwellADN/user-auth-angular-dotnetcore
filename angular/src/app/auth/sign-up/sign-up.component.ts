import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {
  errorMessage!: string;
  authForm!: FormGroup;
  passwordError!: string;
  loading: boolean = false;
  passwordIsValid: boolean = false;

  constructor(private formBuilder: FormBuilder, private authService: AuthService,private router: Router) {
    this.authForm = this.formBuilder.group({
      fullname: ['', [Validators.required]],
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
      agreeTerm: false
    });
  }

  ngOnInit(): void {
  }

  signUp(user: User){
    if (!this.authForm.valid) return;

    if (user.agreeTerm === false){
      //Disable loader
      this.loading = false;
      this.errorMessage = "You must agree to terms & conditions before signing up!";
      return;
    }
    //Enable loader
    this.loading = true;
    //Set user role id to 1 for admin role
    user.roleId = 1;
    return this.authService.signUp(user).subscribe(
      user => {
        //Add user data to local storage
        localStorage.setItem('user', JSON.stringify(user));
        // Redirect user to protected route
        this.router.navigateByUrl('/');
      },error => {
        // If error
        console.log(error);
        //Show error message to user
        //If user already exist, 409 => Conflit
        if (error.status === 409){
          this.errorMessage = "This email is already registered!";
        }else{
          this.errorMessage = "Something went wrong. Please try again!";
        }
        //Disable loader
        this.loading = false;
      }
    );
  }

}
