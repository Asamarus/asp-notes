import { test, expect } from '@playwright/test'
import { login, addNewSection, deleteNote } from '../common/utils'
import { userCredentials, mainUrl } from '../common/config'

test.describe('Notes CRUD', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, userCredentials.email, userCredentials.password)
    await addNewSection(page, 'section1', 'Section1', '#000000')
    await addNewSection(page, 'section2', 'Section2', '#000000')
    await page.goto(`${mainUrl}notes/section1`)
  })

  test('notes CRUD', async ({ page }) => {
    //Add new note
    await page.getByRole('button', { name: 'Add new note' }).click()

    // Update note
    await page.getByRole('textbox', { name: 'Title' }).click()
    await page.getByRole('textbox', { name: 'Title' }).fill('Test')
    await page.getByRole('paragraph').click()
    await page.getByRole('paragraph').fill('Test content')
    await page.getByRole('button', { name: 'Save' }).click()
    await expect(page.getByText('Note is updated!')).toBeVisible()

    await deleteNote(page)
  })

  test('note book CRUD', async ({ page }) => {
    await page.getByRole('button', { name: 'Add new note' }).click()

    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(2).click()
    await page.getByRole('textbox', { name: 'Search books' }).click()
    await page.getByRole('textbox', { name: 'Search books' }).fill('my book')
    await page.getByRole('option', { name: '+ Create new "my book" book' }).click()
    await page.getByRole('button', { name: "Update note's book" }).click()
    await expect(page.getByText('Note book is updated!')).toBeVisible()

    await deleteNote(page)
  })

  test('note tags CRUD', async ({ page }) => {
    await page.getByRole('button', { name: 'Add new note' }).click()

    await page.getByRole('dialog').getByRole('button').filter({ hasText: /^$/ }).nth(3).click()
    await page.getByRole('textbox', { name: 'Search tags' }).click()
    await page.getByRole('textbox', { name: 'Search tags' }).fill('tag1')
    await page.getByRole('option', { name: '+ Create new "tag1" tag' }).click()
    await page.getByRole('textbox', { name: 'Search tags' }).fill('tag2')
    await page.getByRole('option', { name: '+ Create new "tag2" tag' }).click()
    await page.getByRole('textbox', { name: 'Search tags' }).fill('tag3')
    await page.getByRole('option', { name: '+ Create new "tag3" tag' }).click()
    await page.getByRole('button', { name: "Update note's tags" }).click()

    await expect(page.getByText('Note tags are updated!')).toBeVisible()

    await deleteNote(page)
  })

  test('note sources CRUD', async ({ page }) => {
    await page.getByRole('button', { name: 'Add new note' }).click()

    await page.getByRole('dialog').press('Tab')
    await page.keyboard.press('Space')
    await page.getByRole('menuitem', { name: 'Edit Sources' }).click()
    await page.getByRole('button', { name: 'Add new source' }).click()
    await page.getByRole('textbox', { name: 'Link' }).click()
    await page.getByRole('textbox', { name: 'Link' }).fill('http://example.com/')
    await page.getByRole('button', { name: 'Add', exact: true }).click()
    await expect(page.getByText('New source is added!')).toBeVisible()
    await page.getByLabel('Add new source').getByRole('button').filter({ hasText: /^$/ }).click()
    await page
      .getByRole('dialog', { name: /Edit note #\d+ sources/ })
      .getByRole('button')
      .nth(3)
      .click()
    await page.getByRole('textbox', { name: 'Description' }).click()
    await page.getByRole('textbox', { name: 'Description' }).fill('Test')
    await page
      .getByLabel(/Edit source: #[a-z0-9-]+/)
      .getByRole('button', { name: 'Save' })
      .click()
    await expect(page.getByText('Source is updated!')).toBeVisible()
    await page
      .getByLabel(/Edit source: #[a-z0-9-]+/)
      .getByRole('button')
      .filter({ hasText: /^$/ })
      .click()
    await page
      .getByRole('dialog', { name: /Edit note #\d+ sources/ })
      .getByRole('button')
      .nth(3)
      .click()
    await page
      .getByLabel(/Edit source: #[a-z0-9-]+/)
      .getByRole('button')
      .filter({ hasText: /^$/ })
      .click()
    await page
      .getByRole('dialog', { name: /Edit note #\d+ sources/ })
      .getByRole('button')
      .nth(4)
      .click()
    await page.getByRole('button', { name: 'Confirm' }).click()
    await expect(page.getByText('Source is deleted!')).toBeVisible()

    await page.getByRole('banner').getByRole('button').click()

    await deleteNote(page)
  })

  test('test note section change', async ({ page }) => {
    await page.getByRole('button', { name: 'Add new note' }).click()
    await page.getByRole('dialog').press('Tab')
    await page.keyboard.press('Space')
    await page.getByRole('menuitem', { name: 'Change section' }).click()

    await page.getByRole('textbox', { name: 'Section' }).click()
    await page.getByRole('option', { name: 'Section2' }).click()
    await page.getByLabel('Change note #47 section').getByRole('button', { name: 'Save' }).click()
    await expect(page.getByText('Note section is changed!')).toBeVisible()
  })
})
