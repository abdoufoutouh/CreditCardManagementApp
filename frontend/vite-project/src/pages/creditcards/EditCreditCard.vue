<template>
  <DashboardLayout title="Edit Credit Card" username="User">
    <div class="edit-credit-card-page">
      <div class="page-header">
        <Button 
          variant="ghost" 
          size="small" 
          @click="goBack"
          class="back-button"
        >
          ← Back to Dashboard
        </Button>
        <h1 class="page-title">Edit Credit Card</h1>
        <p class="page-description">
          Update your credit limit and current balance below.
        </p>
      </div>

      <div v-if="isLoadingCard" class="loading-state">
        <div class="spinner"></div>
        <p>Loading card details...</p>
      </div>

      <div v-else-if="cardNotFound" class="error-state">
        <p>Credit card not found.</p>
        <Button @click="goBack" variant="primary">Back to Dashboard</Button>
      </div>

      <div v-else class="form-container">
        <EditCreditCardForm 
          :card="cardData"
          @submit="handleSubmit"
          @error="handleError"
          ref="editCardForm"
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
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import DashboardLayout from '../../components/layout/DashboardLayout.vue';
import EditCreditCardForm from '../../components/forms/EditCreditCardForm.vue';
import Button from '../../components/common/Button.vue';
import { useCreditCardAPI } from '../../composables/useCreditCardAPI';

const router = useRouter();
const route = useRoute();
const { getCreditCardById, updateCreditCard } = useCreditCardAPI();

const cardData = ref(null);
const isLoadingCard = ref(true);
const cardNotFound = ref(false);
const showSuccessToast = ref(false);
const showErrorToast = ref(false);
const errorMessage = ref('');
const successMessage = ref('');
const editCardForm = ref(null);

const cardId = route.params.id;

onMounted(async () => {
  await loadCardData();
});

const loadCardData = async () => {
  isLoadingCard.value = true;
  cardNotFound.value = false;

  try {
    const result = await getCreditCardById(cardId);
    
    if (result.success && result.data) {
      cardData.value = result.data.data || result.data;
    } else {
      cardNotFound.value = true;
    }
  } catch (error) {
    cardNotFound.value = true;
  } finally {
    isLoadingCard.value = false;
  }
};

const goBack = () => {
  router.push('/dashboard');
};

const handleSubmit = async (updatePayload) => {
  try {
    const result = await updateCreditCard(cardId, updatePayload);
    
    if (result.success) {
      successMessage.value = result.data?.message || 'Credit card updated successfully';
      showSuccessToast.value = true;
      
      // Update local card data
      if (result.data?.data) {
        cardData.value = result.data.data;
      }
      
      // Redirect after 2 seconds
      setTimeout(() => {
        hideSuccessToast();
        router.push('/dashboard');
      }, 2000);
    } else {
      errorMessage.value = result.data?.message || result.error || 'Failed to update credit card';
      showErrorToast.value = true;
    }
  } catch (error) {
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
.edit-credit-card-page {
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

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e9ecef;
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.loading-state p {
  font-size: 16px;
  color: #6c757d;
  margin: 0;
}

.error-state {
  background: #fff3cd;
  border: 1px solid #ffc107;
  border-radius: 8px;
  padding: 20px;
  text-align: center;
}

.error-state p {
  font-size: 16px;
  color: #856404;
  margin: 0 0 16px 0;
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
