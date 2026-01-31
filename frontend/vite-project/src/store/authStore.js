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
