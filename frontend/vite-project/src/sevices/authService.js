const API_BASE_URL = 'http://localhost:5249/api';

/**
 * Authentication service for handling API calls
 */
export const authService = {
  /**
   * Sign up a new user
   * @param {Object} userData - User registration data
   * @param {string} userData.firstName
   * @param {string} userData.lastName
   * @param {string} userData.email
   * @param {string} userData.password
   * @returns {Promise<Object>} API response with token and user data
   */
  async signup(userData) {
    const response = await fetch(`${API_BASE_URL}/auth/signup`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(userData),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || 'Signup failed');
    }

    return data;
  },

  /**
   * Login an existing user
   * @param {Object} credentials - User login credentials
   * @param {string} credentials.email
   * @param {string} credentials.password
   * @returns {Promise<Object>} API response with token and user data
   */
  async login(credentials) {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || 'Login failed');
    }

    return data;
  },

  /**
   * Logout (client-side token removal)
   * @returns {Promise<Object>} API response
   */
  async logout() {
    const token = localStorage.getItem('token');
    
    if (token) {
      try {
        await fetch(`${API_BASE_URL}/auth/logout`, {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
          },
        });
      } catch (error) {
        // Ignore errors on logout
        console.error('Logout API call failed:', error);
      }
    }
  },
};
