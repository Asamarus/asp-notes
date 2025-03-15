import { test, expect } from '@playwright/test'
import { login } from '../common/utils'
import { userCredentials, mainUrl } from '../common/config'

test.describe('Sections', () => {
  test('sections CRUD', async ({ page }) => {
    await login(page, userCredentials.email, userCredentials.password)
    await page.goto(`${mainUrl}settings/sections`)

    // Add new section
    await page.getByRole('button', { name: 'Add new section' }).click()
    await page.getByRole('textbox', { name: 'Name', exact: true }).fill('test')
    await page.getByRole('textbox', { name: 'Display name' }).fill('Test')
    await page.getByRole('textbox', { name: 'Color' }).fill('#000000')
    await page.getByRole('button', { name: 'Add', exact: true }).click()
    await expect(page.getByText('Section is created!')).toBeVisible()

    // Update section
    await page.getByRole('main').getByRole('button').nth(2).click()
    await page.getByRole('textbox', { name: 'Display name' }).fill('Test2')
    await page.getByRole('textbox', { name: 'Color' }).fill('#303035')
    await page.getByRole('button', { name: 'Save' }).click()
    await expect(page.getByText('Section is updated!')).toBeVisible()

    // Delete section
    await page.getByRole('button').filter({ hasText: /^$/ }).nth(4).click()
    await expect(page.getByRole('dialog')).toContainText(
      'Are you sure you want to delete this section?',
    )
    await page.getByRole('button', { name: 'Cancel' }).click()
    await page.getByRole('button').filter({ hasText: /^$/ }).nth(4).click()
    await page.getByRole('button', { name: 'Confirm' }).click()
    await expect(page.getByText('Section is deleted!')).toBeVisible()
  })

  test('sections form validation', async ({ page }) => {
    await login(page, userCredentials.email, userCredentials.password)
    await page.goto(`${mainUrl}settings/sections`)

    await page.getByRole('button', { name: 'Add new section' }).click()
    await page.getByRole('button', { name: 'Add', exact: true }).click()
    await expect(page.getByText('Name is required!', { exact: true })).toBeVisible()
    await expect(page.getByText('Display name is required!')).toBeVisible()
    await expect(page.getByText('Color is required!')).toBeVisible()
  })
})
