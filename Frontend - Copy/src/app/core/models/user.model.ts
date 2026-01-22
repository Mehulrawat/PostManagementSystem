export interface LoginResponse {
  token: string;
  username: string;
  roles: string[];
}
export enum UserRole {
  Admin = 'Admin',
  SuperAdmin = 'SuperAdmin',
  User = 'User',

}
export interface Users{
  id: string;
  userName: string,
  email:string
  isActive:boolean
  isAutoDeactivated:boolean
  createdAt:string,
  roles:string[]


}