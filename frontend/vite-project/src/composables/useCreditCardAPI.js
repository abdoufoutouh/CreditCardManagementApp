import { ref } from 'vue';

const API_BASE_URL = 'http://localhost:5249/api';

export function useCreditCardAPI() {
  const isLoading = ref(false);
  const error = ref(null);

  const getAuthHeaders = () => {
    const token = localStorage.getItem('token');
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` })
    };
  };

  const createCreditCard = async (cardData) => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch(`${API_BASE_URL}/creditcard`, {
        method: 'POST',
        headers: getAuthHeaders(),
        body: JSON.stringify(cardData)
      });

      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        return {
          success: false,
          error: errorMessage,
          status: response.status
        };
      }

      return {
        success: responseData?.success ?? true,
        message: responseData?.message || 'Credit card created successfully',
        data: responseData?.data || responseData,
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  const getCreditCards = async () => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch(`${API_BASE_URL}/creditcard`, {
        method: 'GET',
        headers: getAuthHeaders()
      });

      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        return {
          success: false,
          error: errorMessage,
          status: response.status
        };
      }

      return {
        success: responseData?.success ?? true,
        message: responseData?.message || 'Credit cards retrieved successfully',
        data: responseData?.data?.cards || responseData?.data || [],
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  const getCreditCardById = async (cardId) => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch(`${API_BASE_URL}/creditcard/${cardId}`, {
        method: 'GET',
        headers: getAuthHeaders()
      });

      // Handle empty response or non-JSON response
      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        throw new Error(errorMessage);
      }

      return {
        success: true,
        data: responseData || null,
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  const updateCreditCard = async (cardId, cardData) => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch(`${API_BASE_URL}/creditcard/${cardId}`, {
        method: 'PUT',
        headers: getAuthHeaders(),
        body: JSON.stringify(cardData)
      });

      // Handle empty response or non-JSON response
      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        return {
          success: false,
          data: responseData || { message: errorMessage },
          error: errorMessage,
          status: response.status
        };
      }

      return {
        success: responseData?.success ?? true,
        data: responseData || { message: 'Credit card updated successfully' },
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  const deleteCreditCard = async (cardId) => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch(`${API_BASE_URL}/creditcard/${cardId}`, {
        method: 'DELETE',
        headers: getAuthHeaders(),
        body: JSON.stringify({ id: cardId })
      });

      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        return {
          success: false,
          error: errorMessage,
          status: response.status
        };
      }

      return {
        success: responseData?.success ?? true,
        message: responseData?.message || 'Credit card deleted successfully',
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  const searchCreditCards = async (cardType, cardNumber) => {
    isLoading.value = true;
    error.value = null;

    try {
      const params = new URLSearchParams();
      if (cardType) params.append('cardType', cardType);
      if (cardNumber) params.append('cardNumber', cardNumber);

      const response = await fetch(`${API_BASE_URL}/creditcard/search?${params.toString()}`, {
        method: 'GET',
        headers: getAuthHeaders()
      });

      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        return {
          success: false,
          error: errorMessage,
          status: response.status
        };
      }

      return {
        success: responseData?.success ?? true,
        message: responseData?.message || 'Search completed successfully',
        data: responseData?.data?.cards || responseData?.data || [],
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  const clearError = () => {
    error.value = null;
  };

  const generateCardNumber = async (cardType) => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch(`${API_BASE_URL}/creditcard/generate?cardType=${cardType}`, {
        method: 'GET',
        headers: getAuthHeaders()
      });

      let responseData = null;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          responseData = await response.json();
        } catch (jsonError) {
          responseData = null;
        }
      }

      if (!response.ok) {
        const errorMessage = responseData?.message || 
                           response.statusText || 
                           `HTTP error! status: ${response.status}`;
        return {
          success: false,
          error: errorMessage,
          status: response.status
        };
      }

      return {
        success: responseData?.success ?? true,
        cardNumber: responseData?.cardNumber || '',
        cardType: responseData?.cardType || cardType,
        message: responseData?.message || 'Card number generated successfully',
        status: response.status
      };
    } catch (err) {
      error.value = err.message;
      return {
        success: false,
        error: err.message,
        status: err.status || 500
      };
    } finally {
      isLoading.value = false;
    }
  };

  return {
    isLoading,
    error,
    createCreditCard,
    getCreditCards,
    getCreditCardById,
    updateCreditCard,
    deleteCreditCard,
    generateCardNumber,
    searchCreditCards,
    clearError
  };
}
