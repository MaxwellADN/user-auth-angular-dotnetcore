import { Role } from "./role";
import { Tenant } from "./tenant";

export class User {
  id!: number;
  fullname!: string;
  email!: string;
  password!: string;
  passwordConfirmation!: string;
  agreeTerm!: boolean;
  createdAt!: string;
  updatedAt!: string | null;
  rememberMe!: boolean;
  token!: string;
  appOriginUrl!: string;
  roleId!: number;
  role!: Role;
  tenantId!: number;
  tenant!: Tenant;
}
