import { defineStore } from 'pinia';
import { authService } from '../sevices/authService';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    user: null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    /**
     * Decode JWT token to extract user info
     */
    decodeToken(token) {
      try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map((c) => {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
      } catch (error) {
        console.error('Failed to decode token:', error);
        return null;
      }
    },

    /**
     * Initialize auth state from stored token
     */
    initializeAuth() {
      if (this.token && !this.user) {
        const decoded = this.decodeToken(this.token);
        if (decoded) {
          this.user = {
            userId: decoded.sub || decoded.userId,
            email: decoded.email,
            firstName: decoded.firstName,
            lastName: decoded.lastName,
          };
        }
      }
    },

    /**
     * Login user and store token
     */
    async login(email, password) {
      try {
        const response = await authService.login({ email, password });
        
        if (response.success && response.data) {
          this.token = response.data.token;
          this.user = {
            userId: response.data.userId,
            email: response.data.email,
            firstName: response.data.firstName,
            lastName: response.data.lastName,
          };
          
          localStorage.setItem('token', this.token);
          return { success: true };
        }
        
        throw new Error(response.message || 'Login failed');
      } catch (error) {
        return { success: false, error: error.message };
      }
    },

    /**
     * Signup user (does not auto-login, user must login separately)
     */
    async signup(firstName, lastName, email, password) {
      try {
        const response = await authService.signup({
          firstName,
          lastName,
          email,
          password,
        });
        
        if (response.success) {
          // Don't store token - user needs to login separately
          return { success: true };
        }
        
        throw new Error(response.message || 'Signup failed');
      } catch (error) {
        return { success: false, error: error.message };
      }
    },

    /**
     * Logout user and clear token
     */
    async logout() {
      await authService.logout();
      this.token = null;
      this.user = null;
      localStorage.removeItem('token');
    },
  },
});
