<template>
  <DashboardLayout title="Create Credit Card" username="User">
    <div class="create-credit-card-page">
      <div class="page-header">
        <Button 
          variant="ghost" 
          size="small" 
          @click="goBack"
          class="back-button"
        >
          ← Back to Dashboard
        </Button>
        <h1 class="page-title">Create New Credit Card</h1>
        <p class="page-description">
          Fill in the details below to create a new credit card for your account.
        </p>
      </div>

      <div class="form-container">
        <CreditCardForm 
          @submit="handleSubmit"
          @error="handleError"
          ref="creditCardForm"
        />
      </div>

      <!-- Success Toast -->
      <div v-if="showSuccessToast" class="toast success-toast">
        <div class="toast-content">
          <div class="toast-icon">✅</div>
          <div class="toast-message">
            <strong>Success!</strong>
            <p>{{ successMessage }}</p>
          </div>
        </div>
        <button @click="hideSuccessToast" class="toast-close">×</button>
      </div>

      <!-- Error Toast -->
      <div v-if="showErrorToast" class="toast error-toast">
        <div class="toast-content">
          <div class="toast-icon">❌</div>
          <div class="toast-message">
            <strong>Error!</strong>
            <p>{{ errorMessage }}</p>
          </div>
        </div>
        <button @click="hideErrorToast" class="toast-close">×</button>
      </div>
    </div>
  </DashboardLayout>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import DashboardLayout from '../../components/layout/DashboardLayout.vue';
import CreditCardForm from '../../components/forms/CreditCardForm.vue';
import Button from '../../components/common/Button.vue';
import { useCreditCardAPI } from '../../composables/useCreditCardAPI';

const router = useRouter();
const { createCreditCard, isLoading } = useCreditCardAPI();

const creditCardForm = ref(null);
const showSuccessToast = ref(false);
const showErrorToast = ref(false);
const errorMessage = ref('');
const successMessage = ref('');

const goBack = () => {
  router.push('/dashboard');
};

const handleSubmit = async (cardData) => {
  try {
    console.log('Submitting card data:', cardData);
    const result = await createCreditCard(cardData);
    console.log('API result:', result);
    
    if (result.success) {
      successMessage.value = result.message || 'Credit card created successfully';
      showSuccessToast.value = true;
      creditCardForm.value?.resetForm();
      
      // Hide success toast after 3 seconds and redirect
      setTimeout(() => {
        hideSuccessToast();
        router.push('/dashboard');
      }, 3000);
    } else {
      console.error('API error:', result);
      errorMessage.value = result.error || result.message || 'Failed to create credit card';
      showErrorToast.value = true;
    }
  } catch (error) {
    console.error('Submit error:', error);
    errorMessage.value = error.message || 'An unexpected error occurred';
    showErrorToast.value = true;
  }
};

const handleError = (error) => {
  errorMessage.value = error;
  showErrorToast.value = true;
};

const hideSuccessToast = () => {
  showSuccessToast.value = false;
};

const hideErrorToast = () => {
  showErrorToast.value = false;
};
</script>

<style scoped>
.create-credit-card-page {
  max-width: 100%;
  padding: 0;
}

.page-header {
  margin-bottom: 30px;
}

.back-button {
  margin-bottom: 20px;
}

.page-title {
  font-size: 32px;
  font-weight: 700;
  color: #2c3e50;
  margin: 0 0 10px 0;
  line-height: 1.2;
}

.page-description {
  font-size: 16px;
  color: #6c757d;
  margin: 0;
  line-height: 1.5;
}

.form-container {
  background: #f8f9fa;
  border-radius: 12px;
  padding: 20px;
}

/* Toast Styles */
.toast {
  position: fixed;
  top: 20px;
  right: 20px;
  min-width: 300px;
  max-width: 400px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  z-index: 1000;
  animation: slideIn 0.3s ease-out;
}

.toast-content {
  display: flex;
  align-items: flex-start;
  padding: 16px;
  gap: 12px;
}

.toast-icon {
  font-size: 20px;
  flex-shrink: 0;
  margin-top: 2px;
}

.toast-message {
  flex: 1;
}

.toast-message strong {
  display: block;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 4px;
}

.toast-message p {
  font-size: 13px;
  margin: 0;
  line-height: 1.4;
}

.toast-close {
  position: absolute;
  top: 8px;
  right: 8px;
  background: none;
  border: none;
  font-size: 20px;
  cursor: pointer;
  color: #6c757d;
  padding: 4px;
  line-height: 1;
  border-radius: 4px;
  transition: background-color 0.2s ease;
}

.toast-close:hover {
  background-color: #f8f9fa;
}

.success-toast {
  border-left: 4px solid #28a745;
}

.success-toast .toast-icon {
  color: #28a745;
}

.error-toast {
  border-left: 4px solid #dc3545;
}

.error-toast .toast-icon {
  color: #dc3545;
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .page-title {
    font-size: 24px;
  }
  
  .page-description {
    font-size: 14px;
  }
  
  .form-container {
    padding: 16px;
  }
  
  .toast {
    position: fixed;
    top: 10px;
    left: 10px;
    right: 10px;
    min-width: auto;
    max-width: none;
  }
  
  .toast-content {
    padding: 12px;
  }
}

@media (max-width: 640px) {
  .page-header {
    margin-bottom: 20px;
  }
  
  .page-title {
    font-size: 20px;
  }
  
  .page-description {
    font-size: 13px;
  }
}
</style>
